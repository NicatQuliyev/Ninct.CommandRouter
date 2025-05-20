namespace CommandRouter.Abstractions
{
    /// <summary>
    /// Defines a contract for sending requests through pipeline behaviors to their respective handlers.
    /// </summary>
    public interface ICommandPusher
    {
        /// <summary>
        /// Sends a request through the pipeline behaviors to the corresponding handler and returns the handler's response.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response expected from the request handler.</typeparam>
        /// <param name="request">The request object implementing IRequest with the expected response type.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the request processing.</param>
        /// <returns>A Task representing the asynchronous operation, containing the response from the request handler.</returns>
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}