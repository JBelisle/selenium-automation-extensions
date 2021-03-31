using OpenQA.Selenium;

namespace Selenium.Automation.Extensions.WebElementExtensions
{
    public static class StaleElementExtensions
    {
        /// <summary>
        /// Checks whether the element is stale.
        /// </summary>
        /// <param name="element">this</param>
        /// <returns>Returns true is the given element is stale, or false otherwise.</returns>
        public static bool IsStale(this IWebElement element)
        {
            try
            {
                var willThrowAnExceptionWhenStale = element.Enabled;
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return true;
            }
        }
    }
}