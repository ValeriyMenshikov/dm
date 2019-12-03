using System.Threading.Tasks;

namespace DM.Services.Common.Authorization
{
    /// <summary>
    /// Central authorization endpoint
    /// </summary>
    public interface IIntentionManager
    {
        /// <summary>
        /// Tells if current user is allowed to perform certain action
        /// </summary>
        /// <param name="intention">Intended action</param>
        /// <typeparam name="TIntention">Type of intention</typeparam>
        /// <returns>Whether user is allowed to perform the action</returns>
        Task<bool> IsAllowed<TIntention>(TIntention intention)
            where TIntention : struct;

        /// <summary>
        /// Tells if current user is allowed to perform certain action
        /// </summary>
        /// <param name="intention">Intended action</param>
        /// <param name="target">Object of the action</param>
        /// <typeparam name="TIntention">Type of intention</typeparam>
        /// <typeparam name="TTarget">Type of action object</typeparam>
        /// <returns>Whether user is allowed to perform the action</returns>
        Task<bool> IsAllowed<TIntention, TTarget>(TIntention intention, TTarget target)
            where TIntention : struct
            where TTarget : class;

        /// <summary>
        /// Tells if current user is allowed to perform certain action
        /// </summary>
        /// <param name="intention">Intended action</param>
        /// <param name="target">Object of the action</param>
        /// <param name="subTarget">Dependent object of the action</param>
        /// <typeparam name="TIntention">Type of intention</typeparam>
        /// <typeparam name="TTarget">Type of action object</typeparam>
        /// <typeparam name="TSubTarget">Type of action dependent object</typeparam>
        /// <returns>Whether user is allowed to perform the action</returns>
        Task<bool> IsAllowed<TIntention, TTarget, TSubTarget>(
            TIntention intention, TTarget target, TSubTarget subTarget)
            where TIntention : struct
            where TTarget : class
            where TSubTarget : class;

        /// <summary>
        /// Throws the specific exception <see cref="IntentionManagerException"/> if user is not allowed to perform certain action
        /// </summary>
        /// <param name="intention">Intended action</param>
        /// <typeparam name="TIntention">Type of intention</typeparam>
        /// <returns></returns>
        Task ThrowIfForbidden<TIntention>(TIntention intention)
            where TIntention : struct;

        /// <summary>
        /// Throws the specific exception <see cref="IntentionManagerException"/> if user is not allowed to perform certain action
        /// </summary>
        /// <param name="intention">Intended action</param>
        /// <param name="target">Object of the action</param>
        /// <typeparam name="TIntention">Type of intention</typeparam>
        /// <typeparam name="TTarget">Type of action object</typeparam>
        /// <returns></returns>
        Task ThrowIfForbidden<TIntention, TTarget>(TIntention intention, TTarget target)
            where TIntention : struct
            where TTarget : class;

        /// <summary>
        /// Throws the specific exception <see cref="IntentionManagerException"/> if user is not allowed to perform certain action
        /// </summary>
        /// <param name="intention">Intended action</param>
        /// <param name="target">Object of the action</param>
        /// <param name="subTarget">Dependent object of the action</param>
        /// <typeparam name="TIntention">Type of intention</typeparam>
        /// <typeparam name="TTarget">Type of action object</typeparam>
        /// <typeparam name="TSubTarget">Type of action dependent object</typeparam>
        /// <returns></returns>
        Task ThrowIfForbidden<TIntention, TTarget, TSubTarget>(
            TIntention intention, TTarget target, TSubTarget subTarget)
            where TIntention : struct
            where TTarget : class
            where TSubTarget : class;
    }
}