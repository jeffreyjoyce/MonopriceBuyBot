using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace MonopriceBuyBot
{
    public static class SeleniumExtensions
    {
        public static IWebElement FindElementByXPath(this ChromeDriver driver, string xPath)
        {
            return driver.FindElement("xpath", xPath);
        }
        public static ReadOnlyCollection<IWebElement> FindElementsByXPath(this ChromeDriver driver, string xPath)
        {
            return driver.FindElements("xpath", xPath);
        }
    }
    class Program
    {
        private static ChromeDriver _driver;
        static void Click(string xPath)
        {
            var interactionElement = _driver.FindElementByXPath(xPath);
            interactionElement.Click();
        }
        static void Fill(string xPath, string value)
        {
            var input = _driver.FindElementByXPath(xPath);
            input.Clear();
            input.SendKeys(value);
        }
        static void Main(string[] args) // args - email, password, creditCardCCV, firstName
        {
            var email = args[0];
            var password = args[1];
            var creditCardCCV = args[2];
            var firstName = args[3]; // must match whats shown in the menu "My Account" when not logged in "{YOUR_NAME_HERE}'s Account" when logged in
            
            // monoprice product id, and if its a preferred item vs another in the list (if both can be bought buy the preferred one
            // This would require a rework if trying to buy multiple items

            // SPECIFICALLY COMMENTED OUT, THIS IS AN EXAMPLE MAKE SURE YOU DONT AUTOBUY SHIT YOU DONT WANT
            //var itemsToBuy = new (string ID, bool Preferred)[]
            //{
            //    ("42116", true),
            //    ("37887", false)
            //};

            while (true)
            {
                try
                {
                    var options = new ChromeOptions();
                    _driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromSeconds(30));
                    using (_driver)
                    {
                        while (true)
                        {
                            _driver.Url = "https://www.monoprice.com/cart/index";
                            try
                            {
                                if (!_driver.PageSource.Contains($"{firstName.ToLower()}’s Account"))
                                {
                                    _driver.Url = "https://www.monoprice.com/myaccount/myaccount";
                                    Fill("//input[@name='email_address']", email);
                                    Fill("//input[@name='password']", password);
                                    Click("//input[@type='submit']");
                                    continue;
                                }

                                Fill("//input[@placeholder='Product #' and @name='p_id']", itemsToBuy[0].ID);
                                Fill("//input[@placeholder='Product #' and @name='p_id1']", itemsToBuy[1].ID);
                                Click("//a[@role='link' and @title='Add to Cart']");
                                var cartItems = _driver.FindElementsByXPath("//li[@class='mp-cart-item']");
                                while (cartItems.Count > 1)
                                {
                                    foreach (var item in itemsToBuy.Where(item => !item.Preferred))
                                    {
                                        try
                                        {
                                            Click($"//*[@unbxdattr='RemoveFromCart' and @unbxdparam_sku='{item.ID}']");
                                        }
                                        catch (Exception e)
                                        {
                                        }
                                    }

                                    cartItems = _driver.FindElementsByXPath("//li[@class='mp-cart-item']");
                                }

                                if (cartItems.Count == 0 || _driver.PageSource.Contains("Your shopping cart is empty."))
                                    throw new Exception("Cart Empty");
                                Click("//a[@title='Proceed to Checkout']");
                                Fill("//input[@title='CVV']", creditCardCCV);
                                Click("//button[@id='btnCheckoutSubmit']");
                                return;
                            }
                            catch (Exception e)
                            {
                                Thread.Sleep(TimeSpan.FromSeconds(15));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}