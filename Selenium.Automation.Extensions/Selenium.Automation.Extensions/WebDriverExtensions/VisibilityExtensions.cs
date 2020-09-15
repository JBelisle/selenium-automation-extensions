using OpenQA.Selenium;

namespace Selenium.Automation.Extensions.WebDriverExtensions
{
    public static class VisibilityExtensions
    {
        /// <summary>
        /// Moves the element identified by elementSelector into view on the screen.
        /// </summary>
        /// <param name="driver">this</param>
        /// <param name="elementSelector">The selector for the element to move into the viewport.</param>
        public static void ScrollIntoView(this IWebDriver driver, By elementSelector)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({ behavior: 'auto', block: 'center', inline: 'start' });", driver.FindElement(elementSelector));
        }

        /// <summary>
        /// Moves the element into view on the screen.
        /// </summary>
        /// <param name="driver">this</param>
        /// <param name="element">The element to move into the viewport.</param>
        public static void ScrollIntoView(this IWebDriver driver, IWebElement element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({ behavior: 'auto', block: 'center', inline: 'start' });", element);
        }

        /// <summary>
        /// Checks the display status of an element without throwing exceptions for non-existent elements.
        /// </summary>
        /// <param name="driver">this</param>
        /// <param name="elementSelector">The selector for the element to check the visibility of.</param>
        /// <returns>Returns true is the given selector finds an element which is displayed on the page, or false otherwise.</returns>
        public static bool ElementIsDisplayed(this IWebDriver driver, By elementSelector)
        {
            try
            {
                ScrollIntoView(driver, elementSelector);
                var element = driver.FindElement(elementSelector);
                return element.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return ElementIsDisplayed(driver, elementSelector);
            }
        }
    }
}