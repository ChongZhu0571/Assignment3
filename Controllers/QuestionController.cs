using Assignment3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment3.Controllers
{
    public class QuestionController : Controller
    {
       private Assignment3Entities db = new Assignment3Entities();
        // GET: Question
        public ActionResult GetQuestion(int id)
        {
            
            List<Question> questions = db.Questions.ToList();
            List<Answer> answers = db.Answers.ToList();
            //query all answers:
            var allAnswers = (from  a in answers
                              where a.questionID.Equals(id)
                              select new Answer { answerText = a.answerText, answerDT = a.answerDT, username = a.username }).ToList();
            var questionInfo = (from q in questions
                                where q.questionID.Equals(id)
                                select new Question
                                {
                                    questionName = q.questionName,
                                    vote = q.vote,
                                    view = q.view + 1,
                                    questionDT = q.questionDT,
                                    categoryName = getCategoryName(id),
                                    answers = allAnswers
                                }).ToList();   
            return View(questionInfo);
        }

            public string getCategoryName(int id)
        {
            List<Category> categories = db.Categories.ToList();
            List<Question> questions = db.Questions.ToList();
            string name = null;
            var categoryName = from q in questions
                               join c in categories
                               on q.categoryID equals c.CategoryID
                               where q.questionID.Equals(id)
                               select c.CategoryName;
            foreach(string categoryname in categoryName)
            {
                name = categoryname;
            }
            return name;
           
        }
    }
}