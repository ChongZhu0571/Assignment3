using Assignment3.Models;
using System;
using System.Collections.Generic;
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
            List<Question> questions = db.Questions.ToList();
            List<Question> questions2 = new List<Question>();
            questions2 = questions.GetRange(0, questions.Count());
            List<Answer> answers = db.Answers.ToList();
            List<Category> categories = db.Categories.ToList();
            var answeredQuestions = (from q in questions
                                     join a in answers
                                     on q.questionID equals a.questionID 
                                     
                                     select new Question{ 
                                         vote = q.vote, view = q.view, questionID= q.questionID, 
                                         questionName=q.questionName,
                                         questionDT=q.questionDT 
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
                                        select new Question{ questionID = q.questionID ,vote=q.vote, answerCount= a.Value, view= q.view,questionName= q.questionName, categoryName=c.CategoryName, questionDT= q.questionDT }).ToList();
            var newAnsweredQuestions2 = (from q in questions2
                                         join c in categories
                                         on q.categoryID equals c.CategoryID
                                         select new Question { questionID =q.questionID ,vote = q.vote, answerCount = 0, view = q.view, questionName = q.questionName, categoryName = c.CategoryName, questionDT = q.questionDT }).ToList();
            newAnsweredQuestions1.AddRange(newAnsweredQuestions2);
            return View(newAnsweredQuestions1);
        }
    }
}