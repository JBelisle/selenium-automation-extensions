using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenQA.Selenium;
using Polly;
using Polly.Retry;

namespace Selenium.Automation.Extensions.RetryHelpers
{
    /// <summary>
    /// This class houses common Polly retry policies primarily for use in a Selenium web automation solution for validating state of operations.
    /// Because these policies are focused on web operations/page loads, the retry function is limited to a constant rate (defined in seconds) with the default being 0.5 seconds.
    /// </summary>
    public static class RetryPolicies
    {
        /// <summary>
        /// Returns a Polly RetryPolicy to use when retrieving a list of strings which is expected to not be null or empty.
        /// </summary>
        /// <param name="retryCount">The number of times to retry before giving up. Default = 10.</param>
        /// <param name="secondsBetweenAttempts">The number of seconds to wait between retry attempts. Default = 0.5.</param>
        /// <returns>A Polly RetryPolicy that will retry on the result of a list of string being null or empty, or any WebDriverException.</returns>
        public static RetryPolicy<List<string>> GetTextList(int retryCount = 10, double secondsBetweenAttempts = 0.5)
        {
            return Policy
                .HandleResult<List<string>>(x => x == null || !x.Any())
                .Or<WebDriverException>()
                .WaitAndRetry(retryCount, x => TimeSpan.FromSeconds(secondsBetweenAttempts));
        }

        /// <summary>
        /// Returns a Polly RetryPolicy to use when retrieving a list of strings which is expected to not be null or empty and to not have empty strings.
        /// </summary>
        /// <param name="retryCount">The number of times to retry before giving up. Default = 10.</param>
        /// <param name="secondsBetweenAttempts">The number of seconds to wait between retry attempts. Default = 0.5.</param>
        /// <returns>A Polly RetryPolicy that will retry on the result of a list of string being null or empty, or any WebDriverException.</returns>
        public static RetryPolicy<List<string>> GetTextListWithoutEmptyStrings(int retryCount = 10, double secondsBetweenAttempts = 0.5)
        {
            return Policy
                .HandleResult<List<string>>(x => x == null || !x.Any() || x.Any(string.IsNullOrEmpty))
                .Or<WebDriverException>()
                .WaitAndRetry(retryCount, x => TimeSpan.FromSeconds(secondsBetweenAttempts));
        }

        /// <summary>
        /// Returns a Polly RetryPolicy to use when potentially encountering any type of WebDriverException.
        /// </summary>
        /// <param name="retryCount">The number of times to retry before giving up. Default = 30.</param>
        /// <param name="secondsBetweenAttempts">The number of seconds to wait between retry attempts. Default = 0.5.</param>
        /// <returns>A Polly RetryPolicy that will retry on any WebDriverException.</returns>
        public static RetryPolicy HandleDriverException(int retryCount = 30, double secondsBetweenAttempts = 0.5)
        {
            return Policy
                .Handle<WebDriverException>()
                .WaitAndRetry(retryCount, x => TimeSpan.FromSeconds(secondsBetweenAttempts));
        }

        /// <summary>
        /// Returns a Polly RetryPolicy to use when expecting a given boolean result.
        /// </summary>
        /// <param name="conditionToHandle">The boolean condition on which you want to retry. Note, this is the retry condition, not the desired result, so if you want to retry on false, this should be set to false.</param>
        /// <param name="retryCount">The number of times to retry before giving up. Default = 30.</param>
        /// <param name="secondsBetweenAttempts">The number of seconds to wait between retry attempts. Default = 0.5.</param>
        /// <returns>A Polly RetryPolicy that will retry on the result matching the given conditionToHandle.</returns>
        public static RetryPolicy<bool> HandleBoolean(bool conditionToHandle, int retryCount = 30, double secondsBetweenAttempts = 0.5)
        {
            return Policy
                .HandleResult(conditionToHandle)
                .WaitAndRetry(retryCount, x => TimeSpan.FromSeconds(secondsBetweenAttempts));
        }

        /// <summary>
        /// Returns a Polly RetryPolicy to use when expecting a given boolean result which may also encounter any type of WebDriverException.
        /// </summary>
        /// <param name="conditionToHandle"></param>
        /// <param name="retryCount">The number of times to retry before giving up. Default = 30.</param>
        /// <param name="secondsBetweenAttempts">The number of seconds to wait between retry attempts. Default = 0.5.</param>
        /// <returns>A Polly RetryPolicy that will retry on the result matching the given conditionToHandle or any WebDriverException.</returns>
        public static RetryPolicy<bool> HandleBooleanOrDriverException(bool conditionToHandle, int retryCount = 30, double secondsBetweenAttempts = 0.5)
        {
            return Policy
                .HandleResult(conditionToHandle)
                .Or<WebDriverException>()
                .WaitAndRetry(retryCount, x => TimeSpan.FromSeconds(secondsBetweenAttempts));
        }
    }
}
