using Assignment3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment3.Controllers
{
    public class Assignment3_HomeController : Controller
    {
        Assignment3Entities db = new Assignment3Entities();

        // GET: Assignment3_Home
        public ActionResult Index()
        {
            if (Session["username"] != null)
            {
                List<Question> questions = db.Questions.ToList();
                List<Question> questions2 = new List<Question>();
                questions2 = questions.GetRange(0, questions.Count());
                List<Answer> answers = db.Answers.ToList();
                List<Category> categories = db.Categories.ToList();
                var answeredQuestions = (from q in questions
                                         join a in answers
                                         on q.questionID equals a.questionID

                                         select new Question
                                         {
                                             vote = q.vote,
                                             view = q.view,
                                             questionID = q.questionID,
                                             questionName = q.questionName,
                                             questionDT = q.questionDT
                                         }).ToList();


                Dictionary<int, int> answerCount = new Dictionary<int, int>();
                foreach (var item in answeredQuestions.Select(id => id.questionID).Distinct().ToList())
                {
                    var answer = (from a in answers
                                  where a.questionID.Equals(item)
                                  select a.answerID
                                  );

                    answerCount.Add(item, answer.Count());
                    questions2.RemoveAll(id => id.questionID == item);

                }
                var newAnsweredQuestions1 = (from q in questions
                                             join a in answerCount
                                             on q.questionID equals a.Key
                                             join c in categories
                                             on q.categoryID equals c.CategoryID
                                             select new Question { questionID = q.questionID, vote = q.vote, answerCount = a.Value, view = q.view, questionName = q.questionName, categoryName = c.CategoryName, questionDT = q.questionDT }).ToList();
                var newAnsweredQuestions2 = (from q in questions2
                                             join c in categories
                                             on q.categoryID equals c.CategoryID
                                             select new Question { questionID = q.questionID, vote = q.vote, answerCount = 0, view = q.view, questionName = q.questionName, categoryName = c.CategoryName, questionDT = q.questionDT }).ToList();
                newAnsweredQuestions1.AddRange(newAnsweredQuestions2);
                return View(newAnsweredQuestions1);
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }


        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult Categories()
        {
           /* var mostCategoryId = db.Questions.GroupBy(categoryId => categoryId.categoryID)
                .OrderByDescending(rank => rank.Count())
                .Take(10).Select(g => g.Key).ToList();
            List<Category> topCategory = new List<Category>();
            foreach(var categoryId in mostCategoryId)
            {
                Category category = db.Categories.FirstOrDefault(id => id.CategoryID == categoryId);
                topCategory.Add(category);
            }*/
            return View(db.Categories.ToList());
        }
        public ActionResult Questions()
        {
           
            return View(db.Questions.ToList());
        }

        public ActionResult Login()
        {
            if (Session["username"] == null)
            {
                return View();
            }
            else
            {
                return View("Logined");
            }
            
        }

        public ActionResult Logout()
        {
            Session["username"] = null;
            return RedirectToAction("Login");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user,string password2)
        {
            if (ModelState.IsValid)
            {
                if(user.password == password2)
                {
                    var check = db.Users.FirstOrDefault(s => s.username == user.username);
                    if (check == null)
                    {
                        db.Users.Add(user);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.error = "User already exists";
                        return View();
                    }
                }
                else
                {
                    ViewBag.error = "retyped password does not match!";
                    return View();
                }
                


            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                var u = db.Users.Where(s => s.username.Equals(user.username));
                if(u.Count() > 0)
                {
                    var ps = u.Where(p => p.password.Equals(user.password));
                    if (ps.Count() > 0)
                    {
                        Session["username"] = user.username;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.error = "Password is not correct!";
                        return View();
                    }
                }
                else
                {
                    ViewBag.error = "User does not exists!";
                    return View();
                }
            }
            return View();
        }
    }
}