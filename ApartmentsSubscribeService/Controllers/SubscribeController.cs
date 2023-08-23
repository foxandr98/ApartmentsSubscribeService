using ApartmentsSubscribeService.Model;
using ApartmentsSubscribeService.Model.DataBase;
using ApartmentsSubscribeService.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
            using (ApplicationContext db = new ApplicationContext(_logger))
            {
                User? user = db.Users.Include(u => u.Apartments).FirstOrDefault(a => a.Email == email);
                if (user == null)
                {
                    _logger.LogInformation("Email {} не был найден при запросе на список подписок", email);
                    return Ok("Подписок на данный Email не обнаружено!");
                }
                List<string> urls = user.Apartments.Select(a => a.Url).ToList();
                Dictionary<string, string> pricesByUrl = digPricesByUrls(urls);
                subscriptionsByEmail = new SubscribesResponseDTO(email, pricesByUrl);
            }
            _logger.LogInformation("Список подписок для {} был успешно отправлен", email);
            return Ok(subscriptionsByEmail);
        }

        private Dictionary<string, string> digPricesByUrls(List<string> urls)
        {
            Dictionary<string, string> pricesByUrl = new Dictionary<string, string>();
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments(new List<string>() { "headless", "disable-gpu" });
            foreach (string url in urls)
            {
                using (var driver = new ChromeDriver(chromeOptions))
                {
                    string stringPriceWithRubles;
                    _logger.LogInformation("Запуск веб драйвера для парсинга цены на: {}", url);
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    driver.Navigate().GoToUrl(url);

                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    try
                    {
                        var div = wait.Until(ExpectedConditions.ElementIsVisible(
                        By.CssSelector("div.flat-prices__block-current:not(.flat-prices__minimal-payment)"))).Text;
                        stringPriceWithRubles = driver.FindElements(By.ClassName("flat-prices__block-current"))[1].Text;
                    }
                    catch (WebDriverTimeoutException ex)
                    {
                        _logger.LogInformation("Не удалось найти цену на ресурсе: {}", url);
                        stringPriceWithRubles = "Цена не найдена";
                    }
                    finally
                    {
                        sw.Stop();
                    }

                    _logger.LogInformation("Парсинг сайта длился: {} миллисекунд", sw.ElapsedMilliseconds);
                    pricesByUrl.Add(url, stringPriceWithRubles);
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
                    _logger.LogInformation("Пользователь {} добавлен в БД", user.Email);
                }
                else
                {
                    if (user.Apartments.Contains(apartment))
                        return Ok("Подписка на данную квартиру уже оформлена!");
                    user.Apartments.Add(apartment);
                    _logger.LogInformation("Пользователь {} подписался на {}", user.Email, apartment.Url);
                }
                db.SaveChanges();
            }
            return Ok("Вы подписаны!");
        }
    }
}
