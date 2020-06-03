using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Selenium.Automation.Extensions.Enums;
using Selenium.Automation.Extensions.RetryHelpers;

namespace Selenium.Automation.Extensions.WebDriverExtensions
{
    public static class ClickExtensions
    {
        /// <summary>
        /// Click an element and validate the click registered by checking visibility of an element after the click.
        /// Due to the nature of the web, it is not uncommon for an element to be clicked but to not actually register the click properly. This method
        /// clicks a given element and waits to ensure the click registered by checking the visibility (or non-visibility) or another element.
        /// As an example, you could confirm a Save button was clicked by checking for the existence of a confirmation message.
        /// </summary>
        /// <param name="driver">this</param>
        /// <param name="clickElementSelector">The selector for the element to be click.</param>
        /// <param name="confirmationElementSelector">The selector for the element used you confirm the click registered properly.</param>
        /// <param name="confirmElementIsVisible">Determines whether to confirm by visibility or non-visibility of the element. Default = true.</param>
        /// <param name="clickType">Indicates whether the click action should be a single click, double click, or right click on the element. Default = single click.</param>
        public static void ClickAndConfirmByElementVisibility(this IWebDriver driver, By clickElementSelector, By confirmationElementSelector, bool confirmElementIsVisible = true, ClickType clickType = ClickType.Single)
        {
            ClickAndConfirmByCondition(driver, clickElementSelector, () => driver.ElementIsDisplayed(confirmationElementSelector) == confirmElementIsVisible, clickType);
        }

        /// <summary>
        /// Click an element and validate the click registered by checking for a StaleElementReferenceException from an element after the click
        /// (if the element does not appear on the page prior to the click action, ClickAndConfirmByElementVisibility is used instead of waiting for a StaleElementReferenceException).
        /// Due to the nature of the web, it is not uncommon for an element to be clicked but to not actually register the click properly. This method
        /// clicks a given element and waits to ensure the click registered by checking for a StaleElementReferenceException when accessing a given element.
        /// As an example, if the click should refresh a section of the DOM but may result in no actual changes, you could confirm the click by referencing any refreshed element.
        /// </summary>
        /// <param name="driver">this</param>
        /// <param name="clickElementSelector">The selector for the element to be click.</param>
        /// <param name="confirmationElementSelector">The selector for the element used you confirm the click registered properly.</param>
        /// <param name="clickType">Indicates whether the click action should be a single click, double click, or right click on the element. Default = single click.</param>
        public static void ClickAndConfirmByStaleElement(this IWebDriver driver, By clickElementSelector, By confirmationElementSelector, ClickType clickType = ClickType.Single)
        {
            if (!driver.ElementIsDisplayed(confirmationElementSelector))
            {
                ClickAndConfirmByElementVisibility(driver, clickElementSelector, confirmationElementSelector, true, clickType);
                return;
            }

            var initialConfirmationElement = driver.FindElement(confirmationElementSelector);

            ClickAndConfirmByCondition(driver, clickElementSelector, () =>
            {
                try
                {
                    var shouldErrorWhenSuccessful = initialConfirmationElement.Text;
                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    return true;
                }
            }, clickType);
        }

        /// <summary>
        /// Click an element and validate the click registered by checking for an alert box after the click.
        /// Due to the nature of the web, it is not uncommon for an element to be clicked but to not actually register the click properly. This method
        /// clicks a given element and waits to ensure the click registered by checking for the presence of an alert window.
        /// </summary>
        /// <param name="driver">this</param>
        /// <param name="clickElementSelector">The selector for the element you wish to click.</param>
        /// <param name="acceptAlert">Defines whether to accept or dismiss the alert. Null can be passed to leave the alert box active. Default = true.</param>
        public static void ClickAndConfirmByAlert(this IWebDriver driver, By clickElementSelector, bool? acceptAlert = true)
        {
            ClickAndConfirmByCondition(driver, clickElementSelector, () =>
            {
                try
                {
                    if (!acceptAlert.HasValue)
                    {
                        driver.SwitchTo().Alert();
                    }
                    else if (acceptAlert.Value)
                    {
                        driver.SwitchTo().Alert().Accept();
                    }
                    else
                    {
                        driver.SwitchTo().Alert().Dismiss();
                    }

                    return true;
                }
                catch (NoAlertPresentException)
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Click an element and validate the click registered by checking for true as the result of the given function.
        /// Due to the nature of the web, it is not uncommon for an element to be clicked but to not actually register the click properly. This method
        /// clicks a given element and waits to ensure the click registered by checking for true as the result of the given condition function.
        /// </summary>
        /// <param name="driver">this</param>
        /// <param name="clickElementSelector">The selector for the element you wish to click.</param>
        /// <param name="conditionFunc">The function used to trigger retry of the click. Returning 'false' indicates a retry condition.</param>
        /// <param name="clickType">Indicates whether the click action should be a single click, double click, or right click on the element. Default = single click.</param>
        public static void ClickAndConfirmByCondition(this IWebDriver driver, By clickElementSelector, Func<bool> conditionFunc, ClickType clickType = ClickType.Single)
        {
            var hasClickedElement = false;

            RetryPolicies.HandleBooleanOrDriverException(false, 20).Execute(() =>
            {
                try
                {
                    driver.ScrollIntoView(clickElementSelector);
                    var clickElement = driver.FindElement(clickElementSelector);

                    switch (clickType)
                    {
                        case ClickType.Single:
                            clickElement.Click();
                            break;
                        case ClickType.Double:
                            new Actions(driver).DoubleClick(clickElement).Perform();
                            break;
                        case ClickType.Right:
                            new Actions(driver).ContextClick(clickElement).Perform();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(clickType), clickType, null);
                    }
                }
                catch (WebDriverException)
                {
                    if (!hasClickedElement)
                    {
                        throw;
                    }
                }

                hasClickedElement = true;

                return RetryPolicies.HandleBooleanOrDriverException(false, 5).Execute(conditionFunc.Invoke);
            });
        }
    }
}