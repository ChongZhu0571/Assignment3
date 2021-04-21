using Assignment3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

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
            //add a view after clicked
            var getView = db.Questions.FirstOrDefault(QId => QId.questionID == id);
            getView.view += 1;
            db.SaveChanges();
            //query all answers:
            var allAnswers = (from  a in answers
                              where a.questionID.Equals(id)
                              select new Answer { answerText = a.answerText, answerDT = a.answerDT, username = a.username }).ToList();
            var questionInfo = (from q in questions
                                where q.questionID.Equals(id)
                                select new Question
                                {
                                    questionID = q.questionID,
                                    questionName = q.questionName,
                                    vote = q.vote,
                                    view = q.view,
                                    questionDT = q.questionDT,
                                    categoryName = getCategoryName(id),
                                    answers = allAnswers
                                }).ToList();   
            return View(questionInfo);
        }

       [HttpPost]
        public ActionResult AddAnswer(Question question)
        {
            Debug.WriteLine(question.questionID);
            Answer answer = new Answer();
            answer.answerText = question.answer.answerText;
            answer.answerDT = DateTime.Now;
            answer.questionID = question.questionID;
            answer.username = "Zhu";
            db.Answers.Add(answer);
            db.SaveChanges();
            return RedirectToAction("GetQuestion", "Question", new { id=question.questionID});        
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