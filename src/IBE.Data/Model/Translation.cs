﻿/*=====================================================================================

	Interlinear Bible Editor
	.NET Windows Forms Interlinear Bible wysiwyg desktop editor project.
		
    MIT License
    https://github.com/krzysztof-radzimski/InterlinearBibleEditor/blob/main/LICENSE

	Autor: 2009-2021 ITORG Krzysztof Radzimski
	http://itorg.pl

  ===================================================================================*/

using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Linq;

namespace IBE.Data.Model {
    public class Translation : XPObject {
        private string name;
        private string description;
        private string introduction;
        private string chapterString;
        private string chapterPsalmString;
        private Language language;
        private TranslationType type;
        private string detailedInfo;
        private bool catolic;
        private bool recommended;
        private bool openAccess;
        private TheBookType bookType;
        private bool chapterRomanNumbering;
        private bool withStrongs;
        private bool withGrammarCodes;
        private bool hidden;

        public string Name {
            get { return name; }
            set { SetPropertyValue(nameof(Name), ref name, value); }
        }
        public string Description {
            get { return description; }
            set { SetPropertyValue(nameof(Description), ref description, value); }
        }
        public string ChapterString {
            get { return chapterString; }
            set { SetPropertyValue(nameof(ChapterString), ref chapterString, value); }
        }
        public string ChapterPsalmString {
            get { return chapterPsalmString; }
            set { SetPropertyValue(nameof(ChapterPsalmString), ref chapterPsalmString, value); }
        }
        public Language Language {
            get { return language; }
            set { SetPropertyValue(nameof(Language), ref language, value); }
        }

        public TranslationType Type {
            get { return type; }
            set { SetPropertyValue(nameof(Type), ref type, value); }
        }

        public bool Catolic {
            get { return catolic; }
            set { SetPropertyValue(nameof(Catolic), ref catolic, value); }
        }

        public bool OpenAccess {
            get { return openAccess; }
            set { SetPropertyValue(nameof(OpenAccess), ref openAccess, value); }
        }

        public bool Hidden {
            get { return hidden; }
            set { SetPropertyValue(nameof(Hidden), ref hidden, value); }
        }

        public bool Recommended {
            get { return recommended; }
            set { SetPropertyValue(nameof(Recommended), ref recommended, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Introduction {
            get { return introduction; }
            set { SetPropertyValue(nameof(Introduction), ref introduction, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string DetailedInfo {
            get { return detailedInfo; }
            set { SetPropertyValue(nameof(DetailedInfo), ref detailedInfo, value); }
        }

        [Association("BookTranslations")]
        public XPCollection<Book> Books {
            get { return GetCollection<Book>(nameof(Books)); }
        }

        public TheBookType BookType {
            get { return bookType; }
            set { SetPropertyValue(nameof(BookType), ref bookType, value); }
        }

        public bool ChapterRomanNumbering {
            get { return chapterRomanNumbering; }
            set { SetPropertyValue(nameof(ChapterRomanNumbering), ref chapterRomanNumbering, value); }
        }

        public bool WithStrongs {
            get { return withStrongs; }
            set { SetPropertyValue(nameof(WithStrongs), ref withStrongs, value); }
        }
        public bool WithGrammarCodes {
            get { return withGrammarCodes; }
            set { SetPropertyValue(nameof(WithGrammarCodes), ref withGrammarCodes, value); }
        }

        public Translation() : base(new UnitOfWork()) { }
        public Translation(Session session) : base(session) { }

        public string GetTranslatedInfo() {
            if (Type == TranslationType.Interlinear) {
                var translatedBooksText = @"<h4 class=""text-center"">Status tłumaczenia</h4>";
                var allBooksTranslated = !(Books.Where(x => !x.IsTranslated).Any());
                var allChaptersTranslated = true;
                foreach (var book in Books) {
                    var hasNoTranslatedChapters = book.Chapters.Where(x => !x.IsTranslated).Any();
                    if (hasNoTranslatedChapters) {
                        allChaptersTranslated = false;
                        break;
                    }
                }

                if (allBooksTranslated && allChaptersTranslated) {
                    translatedBooksText += $"<p>Tłumaczenie {Name} zostało ukończone.</p>";
                }
                else {
                    translatedBooksText += "<p>Przekład zawiera tłumaczenie:</p><ul>";
                    foreach (var book in Books.OrderBy(x => x.NumberOfBook)) {
                        if (book.IsTranslated) {
                            var firstTranslatedChapter = book.Chapters.Where(x => x.IsTranslated).Select(x => x.NumberOfChapter).Min();
                            translatedBooksText += $@"<li><a href=""/{Name.Replace("+", String.Empty)}/{book.NumberOfBook}/{firstTranslatedChapter}"">{book.BaseBook.BookTitle}</a></i>";
                            var bookTranslationCompleted = !(book.Chapters.Where(x => !x.IsTranslated).Any());
                            if (bookTranslationCompleted) {
                                translatedBooksText += " - księga przetłumaczona w całości";
                            }
                            else {
                                translatedBooksText += "<ul>";
                                var chapters = book.Chapters.OrderBy(x => x.NumberOfChapter);
                                var translationStart = -1;
                                var translationEnd = 0;
                                foreach (var chapter in chapters) {
                                    if (!chapter.IsTranslated) {
                                        if (translationStart != -1 && translationEnd != 0) {
                                            translatedBooksText += "<li>";

                                            var chapterFrom = translationStart.ToString();
                                            if (chapterFrom == "0") { chapterFrom = "prologu"; }

                                            var chapterTo = translationEnd.ToString();
                                            if (chapterTo == "0") { chapterTo = "prologu"; }

                                            if (translationStart != translationEnd) {
                                                translatedBooksText += $@"rozdziały od <a href=""/{Name.Replace("+", String.Empty)}/{book.NumberOfBook}/{translationStart}"">{chapterFrom}</a> do <a href=""/{Name.Replace("+", String.Empty)}/{book.NumberOfBook}/{translationEnd}"">{chapterTo}</a>";
                                            }
                                            else {
                                                translatedBooksText += $@"rozdział <a href=""/{Name.Replace("+", String.Empty)}/{book.NumberOfBook}/{translationEnd}"">{chapterTo}</a>,";
                                            }
                                            translatedBooksText += "</li>";
                                        }

                                        translationStart = -1;
                                        translationEnd = 0;
                                    }
                                    else {
                                        if (translationStart == -1) {
                                            translationStart = chapter.NumberOfChapter;
                                        }
                                        translationEnd = chapter.NumberOfChapter;
                                    }
                                }
                                translatedBooksText += "</ul>";
                            }

                            translatedBooksText += "</li>";
                        }
                    }
                    translatedBooksText += "</ul>";
                }

                return translatedBooksText;
            }
            return default;



            /*
             
            
            @if (Model.Translation.Type == IBE.Data.Model.TranslationType.Interlinear) {
                var translatedBooksText = @"<h4 class=""text-center"">Status tłumaczenia</h4><p>Przekład zawiera tłumaczenie:<br/>";
                foreach (var tBook in Model.Translation.Books) {
                    if (tBook.IsTranslated) {
                        var translatedChapters = tBook.Chapters.Where(x => x.IsTranslated);
                        if (translatedChapters.Count() == tBook.Chapters.Count) {
                            translatedBooksText += $" - <i>{tBook.BaseBook.BookTitle}</i> księga przetłumaczona w całości<br />";
                        }
                        else {
                            var chapterFrom = translatedChapters.Select(x => x.NumberOfChapter).Min().ToString();
                            if (chapterFrom == "0") { chapterFrom = "prologu"; }
                            var chapterTo = translatedChapters.Select(x => x.NumberOfChapter).Max();
                            if (chapterFrom.ToInt() == chapterTo) {
                                translatedBooksText += $" - <i>{tBook.BaseBook.BookTitle}</i> rozdział {chapterTo}<br />";
                            }
                            else {
                                translatedBooksText += $" - <i>{tBook.BaseBook.BookTitle}</i> rozdziały od {chapterFrom} do {chapterTo}<br />";
                            }
                        }
                    }
                }
                translatedBooksText += "</p>";

                @Html.Raw(translatedBooksText)
            }
             
             */
        }
    }

    public enum TranslationType {
        [Description("")]
        None = 0,
        [Description("Przekład interlinearny")]
        Interlinear = 1,
        [Description("Przekład literacki")]
        Default = 2,
        [Description("Przekład dynamiczny")]
        Dynamic = 3,
        [Description("Przekład dosłowny")]
        Literal = 4
    }
}
