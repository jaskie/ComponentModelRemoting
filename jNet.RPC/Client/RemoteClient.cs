﻿//#undef DEBUG

using System;
using System.Collections.Concurrent;
using Newtonsoft.Json.Serialization;

namespace jNet.RPC.Client
{
    public class RemoteClient : MessageController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();               

        public RemoteClient()
        {
            
        }

        protected override void OnDispose()
        {
            base.OnDispose();                       
        }
        
        public ISerializationBinder Binder { set => SetBinder(value); }

        public T GetRootObject<T>()
        {
            try
            {
                var queryMessage = WebSocketMessageCreate(SocketMessage.SocketMessageType.RootQuery, null, null, 0, null);
                var response = SendAndGetResponse<T>(queryMessage).Result;                
                return response;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"From {nameof(GetRootObject)}:");
                throw;
            }
        }

        public T Query<T>(ProxyBase dto, string methodName, params object[] parameters)
        {
            try
            {
                var queryMessage = WebSocketMessageCreate(
                    SocketMessage.SocketMessageType.Query,
                    dto,
                    methodName,
                    parameters.Length,
                    new SocketMessageArrayValue { Value = parameters });
                //Logger.Debug($"Asked for {dto}, method {methodName}");
                return SendAndGetResponse<T>(queryMessage).Result;                
            }
            catch (Exception e)
            {
                Logger.Error("From Query for {0}: {1}", dto, e);
                throw;
            }
        }

        public T Get<T>(ProxyBase dto, string propertyName)
        {
            try
            {
                var queryMessage = WebSocketMessageCreate(
                    SocketMessage.SocketMessageType.Get,
                    dto,
                    propertyName,
                    0,
                    null
                );
                //Logger.Debug($"Asked for {dto}, property {propertyName}");
                return SendAndGetResponse<T>(queryMessage).Result;
            }
            catch (Exception e)
            {
                Logger.Error("From Get {0}: {1}", dto, e);
                throw;
            }
        }

        public void Invoke(ProxyBase dto, string methodName, params object[] parameters)
        {
            Send(WebSocketMessageCreate(
                SocketMessage.SocketMessageType.Invoke,
                dto,
                methodName,
                parameters.Length,
                new SocketMessageArrayValue{Value = parameters}));
        }

        public void Set(ProxyBase dto, object value, string propertyName)
        {
            Send(WebSocketMessageCreate(
                SocketMessage.SocketMessageType.Set,
                dto,
                propertyName,
                1,
                value));
        }

        public void EventAdd(ProxyBase dto, string eventName)
        {
            Send(WebSocketMessageCreate(
                SocketMessage.SocketMessageType.EventAdd,
                dto,
                eventName,
                0,
                null));
        }

        public void EventRemove(ProxyBase dto, string eventName)
        {
            Send(WebSocketMessageCreate(
                SocketMessage.SocketMessageType.EventRemove,
                dto,
                eventName,
                0,
                null));
        }
    }
}
