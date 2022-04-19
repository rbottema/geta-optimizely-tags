﻿// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.DataAccess;
using EPiServer.Security;
using Geta.Optimizely.Tags.Interfaces;
using Geta.Optimizely.Tags.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geta.Optimizely.Tags.Controllers
{
    [Authorize(Roles = "Administrators, WebAdmins, CmsAdmins")]
    public class GetaTagsAdminController : Controller
    {
        public static int PageSize { get; } = 30;

        private readonly ITagRepository _tagRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITagEngine _tagEngine;
        
        public GetaTagsAdminController(
            ITagRepository tagRepository, IContentRepository contentRepository, ITagEngine tagEngine)
        {
            _tagRepository = tagRepository;
            _contentRepository = contentRepository;
            _tagEngine = tagEngine;
        }

        [HttpGet]
        public ActionResult Index(string searchString, int? page)
        {
            var pageNumber = page ?? 1;
            var tags = _tagRepository.GetAllTags().ToList();
            ViewBag.TotalCount = tags.Count;

            if (string.IsNullOrEmpty(searchString) && (page == null || page == pageNumber))
            {
                return View( GetPagedTagList(tags, pageNumber));
            }

            ViewBag.SearchString = searchString;
            tags = _tagRepository.GetAllTags().Where(s => s.Name.Contains(searchString)).ToList();

            return View(GetPagedTagList(tags, pageNumber));
        }

        private TagListViewModel GetPagedTagList(IList<Tag> tags, int pageNumber)
        {
            var viewModel = new TagListViewModel
            {
                PageNumber = pageNumber,
                PageCount = (tags.Count + PageSize -1) / PageSize,
                TotalItemCount = tags.Count,
                Tags = tags.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList()
            };
            return viewModel;
        }

        public ActionResult Edit(string tagId, int? page, string searchString)
        {
            if (tagId == null)
            {
                return NotFound();
            }

            var tag = _tagRepository.GetTagById(Identity.Parse(tagId));
            if (tag == null)
            {
                return NotFound();
            }

            ViewBag.Page = page;
            ViewBag.SearchString = searchString;
            return PartialView(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Tag eddittedTag, int? page, string searchString)
        {
            if (id == null)
            {
                return NotFound();
            }

            var existingTag = _tagRepository.GetTagById(Identity.Parse(id));

            if (existingTag == null)
            {
                return RedirectToAction("Index", new { page, searchString });
            }

            if (eddittedTag.checkedEditContentTags)
            {
                EditTagsInContentRepository(existingTag, eddittedTag);
            }

            existingTag.Name = eddittedTag.Name;
            existingTag.GroupKey = eddittedTag.GroupKey;
            _tagRepository.Save(existingTag);

            return RedirectToAction("Index", new { page, searchString });
        }

        public void EditTagsInContentRepository(Tag tagFromTagRepository, Tag tagFromUser)
        {
            var existingTagName = tagFromTagRepository.Name;
            var contentReferencesFromTag = _tagEngine.GetContentReferencesByTags(existingTagName);

            foreach (var item in contentReferencesFromTag)
            {
                var pageFromRepository = (ContentData)_contentRepository.Get<IContent>(item);

                var clone = pageFromRepository.CreateWritableClone();

                var tagAttributes = clone.GetType().GetProperties().Where(
                   prop => Attribute.IsDefined(prop, typeof(UIHintAttribute)) &&
                   prop.PropertyType == typeof(string) &&
                   Attribute.GetCustomAttributes(prop, typeof(UIHintAttribute)).Any(x => ((UIHintAttribute)x).UIHint == "Tags"));

                foreach (var tagAttribute in tagAttributes)
                {
                    var tags = tagAttribute.GetValue(clone) as string;
                    if (string.IsNullOrEmpty(tags)) continue;

                    var pageTagList = tags.Split(',').ToList();
                    var indexTagToReplace = pageTagList.IndexOf(existingTagName);

                    if (indexTagToReplace == -1) continue;
                    pageTagList[indexTagToReplace] = tagFromUser.Name;

                    var tagsCommaSeperated = string.Join(",", pageTagList);

                    tagAttribute.SetValue(clone, tagsCommaSeperated);
                }
                _contentRepository.Save((IContent)clone, SaveAction.Publish, AccessLevel.NoAccess);
            }
            _tagRepository.Delete(tagFromTagRepository);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string tagId, int? page, string searchString)
        {
            if (tagId != null)
            {
                var existingTag = _tagRepository.GetTagById(Identity.Parse(tagId));

                _tagRepository.Delete(existingTag);

                ViewBag.Page = page;
                ViewBag.SearchString = searchString;
                return RedirectToAction("Index", new { page, searchString });
            }

            return View();
        }
    }
}