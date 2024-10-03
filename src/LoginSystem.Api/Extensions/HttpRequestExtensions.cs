namespace LoginSystem.Api.Extensions
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Gets the protocol used by the original client to make this request.<br/>
        /// If the request has a <a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Forwarded-Proto">X-Forwarded-Proto</a>
        /// header, that is used, otherwise the request scheme is used.<br/>
        /// This means if the request comes via an ingress controller, this will return the protocol used by the client to
        /// communicate with the ingress controller, rather than the one used by the ingress controller to communicate with
        /// the service.
        /// </summary>
        /// <param name="request">The HttpRequest instance.</param>
        /// <returns>
        /// The protocol used by the client for the request.
        /// </returns>
        public static string GetProtocol(this HttpRequest request)
        {
            return request.Headers.TryGetValue("X-Forwarded-Proto", out var protocol)
                ? protocol.ToString()
                : request.Scheme;
        }
    }
}
