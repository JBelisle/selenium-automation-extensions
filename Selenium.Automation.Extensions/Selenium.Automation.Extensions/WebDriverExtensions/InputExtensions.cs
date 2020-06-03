using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Automation.Extensions.Enums;
using Selenium.Automation.Extensions.RetryHelpers;

namespace Selenium.Automation.Extensions.WebDriverExtensions
{
    public static class InputExtensions
    {
        /// <summary>
        /// Returns the current value(s) of a given input field.
        /// </summary>
        /// <param name="driver">this</param>
        /// <param name="inputFieldSelector">The selector for the input field to pull the current value(s) from.</param>
        /// <param name="inputFieldType">The InputFieldType of the input field.</param>
        /// <returns>A list of the current value(s) of the specified input field. All input field types other than a Multi-Select Box will return a list with a single entry.</returns>
        public static List<string> GetInputFieldValues(this IWebDriver driver, By inputFieldSelector, InputFieldType inputFieldType)
        {
            driver.ScrollIntoView(inputFieldSelector);

            if (inputFieldType == InputFieldType.MultiSelectBox)
            {
                var inputField = RetryPolicies.HandleDriverException().Execute(() => driver.FindElement(inputFieldSelector));
                return new SelectElement(inputField).Options.Select(x => x.Text).ToList();
            }

            return new List<string> { GetInputFieldValue(driver, inputFieldSelector, inputFieldType) };
        }

        /// <summary>
        /// Returns the current value of a given input field.
        /// </summary>
        /// <param name="driver">this</param>
        /// <param name="inputFieldSelector">The selector for the input field to pull the current value from.</param>
        /// <param name="inputFieldType">The InputFieldType of the input field.</param>
        /// <returns>The current value of the specified input field.</returns>
        public static string GetInputFieldValue(this IWebDriver driver, By inputFieldSelector, InputFieldType inputFieldType)
        {
            driver.ScrollIntoView(inputFieldSelector);
            var inputField = RetryPolicies.HandleDriverException().Execute(() => driver.FindElement(inputFieldSelector));

            switch (inputFieldType)
            {
                case InputFieldType.Text:
                case InputFieldType.Password:
                    return inputField.GetAttribute("value");
                case InputFieldType.Dropdown:
                case InputFieldType.MultiSelectBox:
                    var selectElement = new SelectElement(inputField);
                    return selectElement.SelectedOption.Text;
                case InputFieldType.Checkbox:
                    return inputField.Selected ? "true" : "false";
                default:
                    throw new ArgumentOutOfRangeException($"{inputFieldType} is not a valid input field type.");
            }
        }

        /// <summary>
        /// Sets the value of an input field.
        /// </summary>
        /// <param name="driver">this</param>
        /// <param name="inputFieldSelector">The selector for the input field to populate.</param>
        /// <param name="inputFieldType">The InputFieldType of the input field.</param>
        /// <param name="value">The value with which to populate the specified input field.</param>
        public static void SetInputFieldValue(this IWebDriver driver, By inputFieldSelector, InputFieldType inputFieldType, string value)
        {
            driver.ScrollIntoView(inputFieldSelector);
            var inputField = RetryPolicies.HandleDriverException().Execute(() => driver.FindElement(inputFieldSelector));

            switch (inputFieldType)
            {
                case InputFieldType.Text:
                case InputFieldType.Password:
                    inputField.Clear();
                    inputField.SendKeys(value);
                    break;
                case InputFieldType.Dropdown:
                case InputFieldType.MultiSelectBox:
                    var selectElement = new SelectElement(inputField);
                    selectElement.SelectByText(value);
                    break;
                case InputFieldType.Checkbox:
                case InputFieldType.RadioButton:
                    var currentValue = inputField.Selected ? "true" : "false";
                    if (!currentValue.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        inputField.Click();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{inputFieldType} is not a valid input field type.");
            }
        }
    }
}