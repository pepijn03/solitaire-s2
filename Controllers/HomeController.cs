using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Solitaire_S2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SolitaireLogicLayer;
using Microsoft.AspNetCore.Http;
using SolitaireLogicLayer.Containers;
using SolitaireDataAccessLayer;
using SolitaireInterfaceLayer;
using System.Text.Json;

namespace Solitaire_S2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly User user;
        private IUserContainerDAL userDal;
        private UserContainer userContainer;

        public HomeController(ILogger<HomeController> logger, IUserContainerDAL userInterface)
        {
            userDal = userInterface;

            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Game()
        {
            GameBoard gameBoard = new GameBoard();
            gameBoard.GameStart();
            CardDealer dealer = new CardDealer();
            dealer.DealCards(gameBoard);
            //Stack stack = new Stack();
            //if(gameBoard.GameStateActive == true)
            //{
            //    stack.GetAllCards();
            //}
            ViewData["cards"] = JsonSerializer.Serialize(gameBoard.stacks[0]);
            return View(gameBoard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login()
        {
            ViewBag.Message = "";

            if (user.Name == "")
            {
                ViewBag.Message = "Name Required";
                return View(user);
            }

            if (user.Password == "")
            {
                ViewBag.Message = "Password required";

                return View(user);
            }

            if (ModelState.IsValid)
            {

                bool credCorrect = userDal.CheckLoginCorrect(user.UserToUserDTO(user));

                if (credCorrect == true)
                {
                    //string userObject = JsonConvert.SerializeObject(userDal.GetUserByID(user.UserID));


                    HttpContext.Session.SetInt32("id", Convert.ToInt32(credCorrect));
                    //HttpContext.Session.SetString("User", userObject);

                    long userID = (long)HttpContext.Session.GetInt32("id");
                    User user1 = new User(userDal.GetUserByID(user.UserID));
                    HttpContext.Session.SetString("Name", user1.Name);

                    //if (user1.TwoFaEnabled == Convert.ToDateTime("1900-01-01 00:00:00"))
                    //{
                    //    return RedirectToAction("TwoFactor");
                    //}

                    return RedirectToAction("AccountPage", "User");
                }
                else
                {
                    ViewBag.Message = "Username or password is incorrect";
                }
            }
            return View(user);
        }

        public IActionResult Account()
        {
            User user = new User();
            UserContainer userContainer = new UserContainer(userDal);
            user = userContainer.GetUser(1);
            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
