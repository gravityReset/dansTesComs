﻿using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using DansTesComs.Core.Helpers;
using DansTesComs.Ressources.CommentaireExterne;
using DansTesComs.Ressources.General;
using DansTesComs.WebSite.Filters;
using DansTesComs.Core.Models;
using PagedList;
using WebMatrix.WebData;

namespace DansTesComs.WebSite.Controllers
{
    [InitializeSimpleMembership]
    public class CommentaireExternesController : Controller
    {
        public const int PageSize = 10;
        private DansTesComsEntities db = new DansTesComsEntities();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Index(int page = 1)
        {
            var commentaireExternes = db.CommentaireExternes.Where(c => c.IsValide).ToList()
                .OrderByDescending(com => com.DatePost).ToPagedList(page, PageSize);
            ViewBag.TitreContenu = CommentaireExterneRessources.NouveauCommentaires;
            return View(commentaireExternes);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Admin()
        {
            var commentaireExternes = db.CommentaireExternes;
            return View(commentaireExternes.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentaireExterne commentaireExterne = db.CommentaireExternes.Find(id);
            if (commentaireExterne == null)
            {
                return HttpNotFound();
            }
            return View(commentaireExterne);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Create()
        {
            var com = new CommentaireExterne();
            com.CommentairesExterneContents.Add(new CommentairesExterneContent { Commentaire = string.Empty, Pseudo = string.Empty, Niveau = 0 });
            ViewBag.Categories = new SelectList(db.Categories, "Id", "Name", com.Categories);
            return View(com);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commentaireExterne"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CommentairesExterneContents,Lien,Titre,Publication,Categories")] CommentaireExterne commentaireExterne)
        {
            commentaireExterne.RemoveEmptyComs();
            commentaireExterne.DatePost = DateTime.Now;
            commentaireExterne.PosterUserId = WebSecurity.CurrentUserId;
            commentaireExterne.lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (ModelState.IsValid)
            {
                try
                {
                    db.CommentaireExternes.Add(commentaireExterne);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    ViewBag.ErreurCommentaireContent = string.Join("|",
                        ex.EntityValidationErrors.Select(
                            e => string.Join("-", e.ValidationErrors.Select(x => x.ErrorMessage).ToArray())));
                    return View(commentaireExterne);
                }
                catch (Exception exception)
                {
                    return View(commentaireExterne);
                }

                return RedirectToAction("Index");
            }

            if (commentaireExterne.CommentairesExterneContents.Count == 0)
            {
                ViewBag.ErreurCommentaireContent = CommentaireExterneRessources.CommentairesContentMin;
            }

            return View(commentaireExterne);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Modo")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentaireExterne commentaireExterne = db.CommentaireExternes.Find(id);
            if (commentaireExterne == null)
            {
                return HttpNotFound();
            }
            ViewBag.PosterUserId = new SelectList(db.Users, "Id", "Mail", commentaireExterne.PosterUserId);
            return View(commentaireExterne);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commentaireExterne"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Modo")]
        public ActionResult Edit([Bind(Include = "Id,Titre,Lien,Publication,CommentairesExterneContents,IsValide")] CommentaireExterne commentaireExterne)
        {
            commentaireExterne.RemoveEmptyComs();

            if (ModelState.IsValid)
            {

                var com  = db.CommentaireExternes.Find(commentaireExterne.Id);
                com.Titre = commentaireExterne.Titre;
                com.Lien = commentaireExterne.Lien;
                com.IsValide = commentaireExterne.IsValide;
                com.Publication = commentaireExterne.Publication;

                //on supprime les ancien com et on ajoute les nouveau
                db.CommentairesExterneContents.RemoveRange(com.CommentairesExterneContents);
                com.CommentairesExterneContents = commentaireExterne.CommentairesExterneContents;
                
                db.SaveChanges(); 
                return RedirectToAction("Details", new {id = com.Id});
            }
            return View(commentaireExterne);
        }

        [Authorize(Roles = "Admin,Modo")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentaireExterne commentaireExterne = db.CommentaireExternes.Find(id);
            if (commentaireExterne == null)
            {
                return HttpNotFound();
            }
            return View(commentaireExterne);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Modo")]
        public ActionResult DeleteConfirmed(int id)
        {
            CommentaireExterne commentaireExterne = db.CommentaireExternes.Find(id);
            db.CommentaireExternes.Remove(commentaireExterne);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// affiche les commentaire de facon aléatoire
        /// </summary>
        /// <returns></returns>
        public ActionResult Random()
        {
            var commentaireExternes = db.CommentaireExternes.Where(c => c.IsValide)
                .OrderBy(c=>Guid.NewGuid()).ToList();

            return View( commentaireExternes);
        }

        /// <summary>
        /// affiche les commentaire par categorie
        /// </summary>
        /// <param name="categorieName"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Categorie(string categorieName,int page = 1)
        {
            var commentaireExternes = db.CommentaireExternes.Where(c=>c.Category.Name == categorieName && c.IsValide ).ToList()
                .OrderByDescending(com => com.DatePost).ToPagedList(page, PageSize);
            ViewBag.TitreContenu = categorieName;
            return View(commentaireExternes);
        }

        /// <summary>
        /// trier par mieux noté
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Top(int page = 1)
        {
            var commentaireExternes = db.CommentaireExternes.Where(c => c.IsValide)
                .ToList().OrderByDescending(TriNote.CoefPlusMoins).ToPagedList(page, PageSize);

            return View(commentaireExternes);
        }

        /// <summary>
        /// trier par moins bien noté
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Flop(int page = 1)
        {
            var commentaireExternes = db.CommentaireExternes.Where(c=>c.IsValide)
                .ToList().OrderBy(TriNote.CoefPlusMoins).ToPagedList(page, PageSize);

            return View(commentaireExternes);
        }
        
        /// <summary>
        /// Les commentaire en attente de validation
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Wait(int page = 1)
        {
            var commentaireExternes = db.CommentaireExternes.Where(c => !c.IsValide)
                .ToList().ToPagedList(page, PageSize);

            return View(commentaireExternes);
        }

        /// <summary>
        /// Les commentaire les plus commentés
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult MostComment(int page = 1)
        {
            var commentaireExternes = db.CommentaireExternes.Where(c => c.IsValide)
                .ToList().OrderByDescending(c=>c.Commentaires.Count).ToPagedList(page, PageSize);

            return View(commentaireExternes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
