﻿using ApartmentsSubscribeService.Model;
using ApartmentsSubscribeService.Model.DataBase;
using ApartmentsSubscribeService.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace ApartmentsSubscribeService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribeController : ControllerBase
    {
        private readonly ILogger<SubscribeController> _logger;
        public SubscribeController(ILogger<SubscribeController> logger)
        {
            _logger = logger;
        }

        // GET: api/subscribe
        [HttpGet]
        public IActionResult Get([EmailAddress] string email)
        {
            SubscribesResponseDTO subscriptionsByEmail;
            using (ApplicationContext db = new ApplicationContext())
            {
                User? user = db.Users.Include(u => u.Apartments).FirstOrDefault(a => a.Email == email);
                if (user == null)
                {
                    return Ok("Подписок на данный Email не обнаружено!");
                }
                List<string> urls = user.Apartments.Select(a => a.Url).ToList();
                Dictionary<string, uint> pricesByUrl = digPricesByUrls(urls);
                subscriptionsByEmail = new SubscribesResponseDTO(email, pricesByUrl);
            }
            return Ok(subscriptionsByEmail);
        }

        private Dictionary<string, uint> digPricesByUrls(List<string> urls)
        {
            Dictionary<string, uint> pricesByUrl = new Dictionary<string, uint>();
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments(new List<string>() { "headless", "disable-gpu" });
            foreach (string url in urls)
            {
                using (var driver = new ChromeDriver(chromeOptions))
                {
                    driver.Navigate().GoToUrl(url);

                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    var div = wait.Until(ExpectedConditions.ElementIsVisible(
                        By.CssSelector("div.flat-prices__block-current:not(.flat-prices__minimal-payment)"))).Text;

                    var stringPriceWithRubles = driver.FindElements(By.ClassName("flat-prices__block-current"))[1].Text;
                    var parsedPrice = Regex.Replace(stringPriceWithRubles, "\\s+|₽", "");
                    pricesByUrl.Add(url, UInt32.Parse(parsedPrice));
                }
            }
            return pricesByUrl;
        }

        // POST api/subscribe
        [HttpPost]
        public IActionResult Post([FromBody] SubscribeRequestDTO subscribe)
        {
            string email = subscribe.Email;
            string url = subscribe.Url;
            User? user;
            Apartment apartment;
            using (ApplicationContext db = new ApplicationContext())
            {
                user = db.Users.Include(u => u.Apartments).FirstOrDefault(u => u.Email == email);
                apartment = db.Apartments.Include(u => u.Users).FirstOrDefault(u => u.Url == url) ?? new Apartment(url);

                if (user == null)
                {
                    user = new User(email);
                    user.Apartments = new List<Apartment> { apartment };
                    db.Users.Add(user);
                }
                else
                {
                    if (user.Apartments.Contains(apartment))
                        return Ok("Подписка на данную квартиру уже оформлена!");
                    user.Apartments.Add(apartment);
                }
                db.SaveChanges();
            }
            return Ok("Вы подписаны!");
        }
    }
}
