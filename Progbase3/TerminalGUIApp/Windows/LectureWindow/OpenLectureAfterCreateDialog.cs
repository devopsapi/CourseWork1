using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp.Windows.LectureWindow
{
    public class OpenLectureAfterCreateDialog : Dialog
    {
        private TemporaryLectureRepository temporaryLectureRepository;
        public bool canBeEditedAndDeleted;
        private ListView allLecturesListView;
        private FrameView frameView;
        private int pageLength = 5;
        private int page = 1;
        private string searchValue = "";
        private bool selecting = false;
        private Button prevPageBtn;
        private Button nextPageBtn;
        private Label pageLbl;
        private Label totalPagesLbl;
        private TextField searchInput;
        private Label nullReferenceLbl = new Label();

        private Course course;
        private List<Lecture> lectures;

        public OpenLectureAfterCreateDialog()
        {
            this.Title = "All lectures";


            allLecturesListView = new ListView(new List<Lecture>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allLecturesListView.OpenSelectedItem += OnOpenLecture;

            frameView = new FrameView("Lectures")
            {
                X = Pos.Percent(15),
                Y = Pos.Percent(20),
                Width = Dim.Fill() - Dim.Percent(15),
                Height = pageLength + 2,
            };
            frameView.Add(allLecturesListView);
            this.Add(frameView);

            Button backBtn = new Button(46, 30,"Back");
            backBtn.Clicked += OnCreateDialogSubmit;
            this.AddButton(backBtn);

            Button addLectureBtn = new Button(30, 20, "Add lecture");
            addLectureBtn.Clicked += OnAddLectureClicked;
            this.AddButton(addLectureBtn);


            prevPageBtn = new Button("Previous page")
            {
                X = Pos.Percent(35),
                Y = Pos.Percent(10),
            };
            pageLbl = new Label("?")
            {
                X = Pos.Right(prevPageBtn) + Pos.Percent(3),
                Y = Pos.Top(prevPageBtn),
                Width = 3,
            };

            Label separateLbl = new Label("of")
            {
                X = Pos.Right(pageLbl) + Pos.Percent(2),
                Y = Pos.Top(pageLbl),
            };
            totalPagesLbl = new Label("?")
            {
                X = Pos.Right(separateLbl) + Pos.Percent(3),
                Y = Pos.Top(pageLbl),
                Width = 3,
            };
            nextPageBtn = new Button("Next page")
            {
                X = Pos.Right(totalPagesLbl) + Pos.Percent(3),
                Y = Pos.Top(prevPageBtn),
            };
            nextPageBtn.Clicked += OnNextPage;
            prevPageBtn.Clicked += OnPrevPage;
            this.Add(prevPageBtn, pageLbl, separateLbl, totalPagesLbl, nextPageBtn);

            Label searchLbl = new Label("Seeking categories - ")
            {
                X = Pos.Percent(33),
                Y = Pos.Percent(15),
            };
            Label chooseSearchColumn = new Label("Topic - ")
            {
                X = Pos.Right(searchLbl),
                Y = Pos.Top(searchLbl),
            };
            searchInput = new TextField()
            {
                X = Pos.Right(chooseSearchColumn) + Pos.Percent(1),
                Y = Pos.Top(searchLbl),
                Width = Dim.Percent(20),
            };
            searchInput.TextChanged += OnSearchChange;
            this.Add(searchLbl, chooseSearchColumn, searchInput);
        }

        public void SetRepository(TemporaryLectureRepository temporaryLectureRepository)
        {
            this.temporaryLectureRepository = temporaryLectureRepository;

            UpdateCurrentPage();
        }

        private void OnSearchChange(NStack.ustring text)
        {
            searchValue = searchInput.Text.ToString();

            UpdateCurrentPage();
        }

        private void OnNextPage()
        {
            int totalPages = this.temporaryLectureRepository.GetSearchPagesCount(pageLength, searchValue);

            if (page >= totalPages)
            {
                return;
            }

            this.page += 1;

            UpdateCurrentPage();
        }

        private void OnPrevPage()
        {
            int totalPages = this.temporaryLectureRepository.GetSearchPagesCount(pageLength, searchValue);

            if (page == 1)
            {
                return;
            }

            this.page -= 1;

            UpdateCurrentPage();
        }


        private void UpdateCurrentPage()
        {
            int totalPages = this.temporaryLectureRepository.GetSearchPagesCount(pageLength, searchValue);

            if (page > totalPages)
            {
                page = 1;
            }

            this.pageLbl.Text = page.ToString();
            this.totalPagesLbl.Text = totalPages.ToString();

            if (!selecting)
            {
                this.allLecturesListView.SetSource(this.temporaryLectureRepository.GetSearchPage(searchValue, page, pageLength));

                if (allLecturesListView.Source.ToList().Count == 0)
                {
                    nullReferenceLbl = new Label("No records found")
                    {
                        X = Pos.Percent(45),
                        Y = Pos.Percent(50),
                    };
                    frameView.RemoveAll();
                    frameView.Add(nullReferenceLbl);
                }
                else
                {
                    frameView.RemoveAll();
                    frameView.Add(allLecturesListView);
                }
            }
            else
            {
                selecting = false;
            }


            prevPageBtn.Visible = (page != 1);
            nextPageBtn.Visible = (page! < totalPages);
        }


        private void OnAddLectureClicked()
        {
            CreateLectureDialog dialog = new CreateLectureDialog();
            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Lecture newLecture = dialog.GetLecture();

                if (newLecture != null)
                {
                    newLecture.id = this.temporaryLectureRepository.Insert(newLecture);
                    UpdateCurrentPage();
                }
            }
        }
        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }

      /*   public void SetLectureList(List<Lecture> lectures)
        {
            this.lectures = lectures;
            this.allLecturesListView.SetSource(lectures);
        } */

        private void OnOpenLecture(ListViewItemEventArgs args)
        {
            Lecture lecture = (Lecture)args.Value;

            OpenLectureDialog dialog = new OpenLectureDialog();

            dialog.SetLecture(lecture);

            dialog.CheckIfLectureCanBeEditedAndDeleted(true);

            Application.Run(dialog);

            if (dialog.edited)
            {
                Lecture editedLecture = dialog.GetLecture();
                editedLecture.id = lecture.id;

                if (editedLecture != null)
                {
                    this.temporaryLectureRepository.Update(editedLecture.id, editedLecture);

                    UpdateCurrentPage();

                    /*     this.lectures[this.lectures.FindIndex(ind => ind.topic.Equals(lecture.topic))] = editedLecture;

                this.allLecturesListView.SetSource(lectures); */
                }
            }

            else if (dialog.deleted)
            {
                this.temporaryLectureRepository.DeleteById(lecture.id);

                UpdateCurrentPage();
                /*  this.lectures.Remove(lecture);

                 this.allLecturesListView.SetSource(lectures); */
            }
        }
    }
}