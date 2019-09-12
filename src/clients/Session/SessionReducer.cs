﻿using Gameye.PublicApi.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Gameye.Sdk
{
    internal class SessionReducer
    { 
        public static SessionState Reduce(SessionState state, JObject action) 
        {
            var sessions = new Dictionary<string, Session>(state.Sessions);
            var payload = action["payload"];

            switch (action["type"].ToString())
            {
                case "session-initialized":
                    var initializedEvent = payload.ToObject<SessionInitializedEventPayload>();
                    sessions.Clear();
                    foreach (var initializedSession in initializedEvent.Sessions)
                    {
                        sessions.Add(initializedSession.Id, new Session
                        {
                            Created = initializedSession.Created,
                            Host = initializedSession.Host,
                            Image = initializedSession.Image, 
                            Location = initializedSession.Location,
                            Port = new Dictionary<string, long>(initializedSession.Port),
                        });
                    }
                    break;

                case "session-started":
                    var startedEvent = payload.ToObject<SessionStartedEventPayload>();
                    var startedSession = startedEvent.Session;
                    if(!sessions.ContainsKey(startedSession.Id))
                    {
                        sessions.Add(startedSession.Id, new Session
                        {
                            Created = startedSession.Created,
                            Host = startedSession.Host,
                            Image = startedSession.Image,
                            Location = startedSession.Location,
                            Port = new Dictionary<string, long>(startedSession.Port),
                        });
                    }
                    break;

                case "session-stopped":
                    var stoppedEvent  = payload.ToObject<SessionStoppedEventPayload>();
                    var stoppedSession = stoppedEvent.Session;
                    if (sessions.ContainsKey(stoppedSession.Id))
                    {
                        sessions.Remove(stoppedSession.Id);
                    }
                    break;
            }

            return SessionState.WithSessions(sessions);
        }
    }
}