using Microsoft.AspNetCore.SignalR;
using System.Runtime.InteropServices;

namespace vocabversus_engine.Hubs.GameHub
{
    /// <summary>
    /// Special <see cref="HubException"> utility for including a parseable errorCode in the error message
    /// </summary>
    public static class GameHubException
    {
        const string gameHubErrorCodeDocs = "https://github.com/Thomas-Molen/VocabVersus-Engine";
        const string gameHubErrorCodeIndicator = "code";

        /// <summary>
        /// Creates <see cref="HubException"> with error message and code
        /// </summary>
        /// <param name="message">error message</param>
        /// <param name="errorCode">error code</param>
        /// <returns><see cref="HubException">with errorcode serialized in message</returns>
        public static HubException Create(string message, int errorCode)
        {
            return new HubException($"{message}\nGameHubErrorCode: <{gameHubErrorCodeIndicator}>{errorCode}</{gameHubErrorCodeIndicator}>\nInformation on this error code can be found here:{gameHubErrorCodeDocs}");
        }

        /// <summary>
        /// Creates <see cref="HubException"> with error message and code
        /// </summary>
        /// <param name="message">error message</param>
        /// <param name="errorCode">error code</param>
        /// <returns><see cref="HubException">with errorcode serialized in message</returns>
        public static HubException Create(string message, GameHubExceptionCode errorCode)
        {
            return Create(message, (int)errorCode);
        }

        /// <summary>
        /// Creates HubException message:
        /// Given identifier was not invalid for operation identifier: {identifier},
        /// errorCode 100
        /// </summary>
        /// <param name="identifier">identifier originating the error</param>
        /// <returns></returns>
        public static HubException CreateIdentifierError(string identifier)
        {
            return Create($"Given identifier was not invalid for operation identifier: {identifier}", GameHubExceptionCode.IdentifierNotFound);
        }
    }
}
