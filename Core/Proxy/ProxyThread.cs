using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using MRI.Core.Message;
using MRI.Core.Utils;
using MRI.Integrity;

namespace MRI.Core.Proxy
{
    public class ProxyThread
    {
        private readonly Socket _client;
        private readonly Socket _mongo;

        private readonly Reader _clientReader;
        private readonly Reader _serverReader;

        private readonly QuerySynchronizer _querySynchronizer;

        public ProxyThread( Socket mongo, Socket client )
        {
            _mongo = mongo;
            _client = client;

            var clientNetworkStream = new NetworkStream( _client );
            var serverNetworkStream = new NetworkStream( _mongo );

            _clientReader = new Reader( clientNetworkStream );
            _serverReader = new Reader( serverNetworkStream );

            _querySynchronizer = QuerySynchronizer.Instance;

            new Thread( Listen ) { IsBackground = true }.Start();
        }

        private void HandleRequest( MongoStandardMessage request )
        {
            _querySynchronizer.StartAction();
            try
            {                
                _mongo.Send( request.RawRequest );    
            }
            finally
            {
                _querySynchronizer.StopAction();                
            }
        }

        private void HandleUpdateRequest(MongoUpdateMessage request)
        {
            _querySynchronizer.StartBlockingAction();
            try
            {
                var referenceBeforeChanges = MongoMessageHandler.GetChanges(request);
                SendRequestToServer(request);

                var checkErrorRequest = ReadClientRequest();
                if (checkErrorRequest.GetType() != typeof(MongoQueryMessage) &&
                    !MessageDataExtractor.IsHasErrorRequest(checkErrorRequest))
                {
                    //TODO: throw error. Und format
                }

                SendRequestToServer(checkErrorRequest);
                var hasErrorResponse = ReadServerResponse();

                if (!MessageDataExtractor.IsHasError(hasErrorResponse))
                {
                    var referenecesAfterChanges = MongoMessageHandler.GetChanges(request);
                    MongoMessageHandler.ApplyUpdateChanges(referenceBeforeChanges, referenecesAfterChanges);
                }

                SendResponseToClient(hasErrorResponse);

            }
            catch (Exception)
            {
                //TODO:
                throw;
            }
            finally
            {
                _querySynchronizer.StopBlockingAction();
            }
        }

        private void HandleDeleteRequest(MongoDeleteMessage request)
        {
            _querySynchronizer.StartBlockingAction();

            try
            {
                var changes = MongoMessageHandler.GetChanges(request);
                SendRequestToServer(request);
                
                var checkErrorRequest = ReadClientRequest();
                if (checkErrorRequest.GetType() != typeof(MongoQueryMessage) &&
                    !MessageDataExtractor.IsHasErrorRequest(checkErrorRequest))
                {
                    //TODO: throw error. Und format
                }

                SendRequestToServer(checkErrorRequest);
                var hasErrorResponse = ReadServerResponse();

                if (!MessageDataExtractor.IsHasError(hasErrorResponse))
                {
                    MongoMessageHandler.ApplyDeleteChanges(changes);
                }

                SendResponseToClient(hasErrorResponse);
            }
            catch (Exception)
            {
                //TODO:
                throw;
            }
            finally
            {
                _querySynchronizer.StopBlockingAction();
            }
        }

        private void HandleInsertRequest(MongoInsertMessage request)
        {
            _querySynchronizer.StartBlockingAction();

            try
            {
                var changes = MongoMessageHandler.GetChanges(request);
                var hasErrors = !MongoMessageHandler.IsInsertAllowed(changes);

                if (!hasErrors)
                {
                    SendRequestToServer(request);    
                }
                
                var checkErrorRequest = ReadClientRequest();

                if (!hasErrors)
                {
                    if (checkErrorRequest.GetType() != typeof (MongoQueryMessage) &&
                        !MessageDataExtractor.IsHasErrorRequest(checkErrorRequest))
                    {
                        //TODO: throw error. Und format
                    }

                    SendRequestToServer(checkErrorRequest);
                    var hasErrorResponse = ReadServerResponse();

                    if (!MessageDataExtractor.IsHasError(hasErrorResponse))
                    {
                        MongoMessageHandler.ApplyInsertChanges(changes);
                    }

                    SendResponseToClient(hasErrorResponse);
                }
                else
                {
                    //send error to client    
                }
            }
            catch (Exception)
            {
                //TODO:
                throw;
            }
            finally
            {
                _querySynchronizer.StopBlockingAction();
            }
        }

        private MongoStandardMessage ReadClientRequest()
        {
            return _clientReader.Read();
        }

        private MongoStandardMessage ReadServerResponse()
        {
            return _serverReader.Read();
        }

        private void SendRequestToServer(MongoStandardMessage clientRequest)
        {
            _mongo.Send(clientRequest.RawRequest);
        }

        private void SendResponseToClient(MongoStandardMessage response)
        {
            _client.Send(response.RawRequest);
        }

        private void Listen()
        {
            try
            {
                while (_client.Connected)
                {
                    var request = ReadClientRequest();

                    if (request.GetType() == typeof (MongoInsertMessage))
                    {
                        HandleInsertRequest((MongoInsertMessage) request);
                    }
                    else if (request.GetType() == typeof (MongoDeleteMessage))
                    {
                        HandleDeleteRequest((MongoDeleteMessage) request);
                    }
                    else if (request.GetType() == typeof (MongoUpdateMessage))
                    {
                        HandleUpdateRequest((MongoUpdateMessage) request);
                    }
                    else
                    {
                        HandleRequest(request);
                        if (request.WaitForResponse())
                        {
                            SendResponseToClient(ReadServerResponse());
                        }
                    }
                }
            }
            catch (EndOfStreamException)
            {
                Console.WriteLine("Connection closed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //TODO: log it
            }
            finally
            {
                if ( _client.Connected )
                    _client.Disconnect( true );
                if ( _mongo.Connected )
                    _mongo.Disconnect( true );
            }
        }
    }
}
