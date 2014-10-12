using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BattleFieldOneCore;
using BattleFieldOne.Models;

namespace BattleFieldOne.Controllers
{
    public class HomeController : Controller
    {
			private GameClass GameData;
			private int TurnPhase;

			public ActionResult Index()
			{
				Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
				Response.Cache.SetLastModified(DateTime.UtcNow.AddDays(0));
				Response.Cache.SetCacheability(HttpCacheability.NoCache);

				if (Request.Form["btnNewGame"] != null)
				{
					Session.Remove("gGameData");
					Response.Redirect(Request.RawUrl);
				}

				if (Session["gGameData"] == null)
				{
					GameData = new GameClass();
					GameData.InitializeGame(1);
					Session["gGameData"] = GameData;
				}
				else
				{
					GameData = (GameClass)Session["gGameData"];
				}

				// move unit ajax call
				if (Request.QueryString["piMoveUnit"] != null)
				{
					int liX = Request.QueryString["pnX"].ToInt();
					int liY = Request.QueryString["pnY"].ToInt();
					int liUnitNumber = Request.QueryString["piMoveUnit"].ToInt();

					GameData.MoveUnit(liUnitNumber, liX, liY);

					Session["gGameData"] = GameData;

					return Content("");
				}

				// next turn ajax call
				if (Request.QueryString["plNextTurn"] != null)
				{
					string returnData = "";

					TurnPhase = Request.QueryString["plNextTurn"].ToInt();

					// check for end of game condition
					string lsEndOfGame = GameData.CheckForEndOfGameCondition();

					if (lsEndOfGame != "")
					{
						returnData += "E|" + lsEndOfGame;
					}
					else if (TurnPhase == 2)
					{
						// enemy movement here
						// figure out which pieces are going to move, then send back the information to javascript to perform the actual movement

						returnData += GameData.CollectGermanMovementData();

						Session["gGameData"] = GameData;
					}
					else if (TurnPhase == 3)
					{
						// enemy attack phase here
						// compute the enemy units that will attack and which allied units they attack, then send back to javascript to visualize the attack

						returnData += GameData.CollectGermanAttackData();

						Session["gGameData"] = GameData;
					}

					return Content(returnData);
				}

				// attack request ajax call
				if (Request.QueryString["piAttackGermanUnit"] != null)
				{
					string returnData = "";

					// return the results of the attack
					//German Unit #, allied unit #, result
					int liGermanUnitNumber = Request.QueryString["piAttackGermanUnit"].ToInt();
					int liAlliedUnitNumber = Request.QueryString["piAlliedUnit"].ToInt();

					string lsReturnData = GameData.AttackGermanUnit(liGermanUnitNumber, liAlliedUnitNumber);

					returnData = lsReturnData;

					Session["gGameData"] = GameData;

					return Content(returnData);
				}

				ViewBag.TurnPhaseDescription = "";
				switch (TurnPhase)
				{
					case 0:
						ViewBag.TurnPhaseDescription = "Allied Movement";
						break;
					case 1:
						ViewBag.TurnPhaseDescription = "Allied Attack";
						break;
					case 2:
						ViewBag.TurnPhaseDescription = "Axis Movement";
						break;
					case 3:
						ViewBag.TurnPhaseDescription = "Axis Attack";
						break;
				}

				ViewBag.GameData = GameData.Render();
				return View();
			}

    }
}
