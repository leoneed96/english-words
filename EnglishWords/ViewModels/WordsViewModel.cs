using Common.WPF.Commands;
using Common.WPF.ViewModels;
using EnglishWords.DAL;
using EnglishWords.Data.DTO;
using EnglishWords.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using ViewModelBase = Common.WPF.ViewModels.ViewModelBase;

namespace EnglishWords.ViewModels
{
    public class WordsViewModel: ViewModelBase
    {
        public WordsViewModel()
        {
            Task.Run(() => GetWordsCommand.Execute(null));
        }
        private List<UpdatedWordDTO> _toSynchronize { get; set; } = new List<UpdatedWordDTO>();
        private ObservableCollection<Word> words;
        private bool _gotData = false;
        public ObservableCollection<Word> Words
        {
            get
            {
                if (words == null)
                    words = new ObservableCollection<Word>();
                return words;
            }

            set
            {
                words = value;
                OnPropertyChanged("Words");
            }
        }

        private ICommand getWordsCommand;
        private ICommand addWordCommand;
        private ICommand validateCommand;
        private ICommand commitUpdateCommand;
        private ICommand rowChangedCommand;
        private ICommand deleteWordCommand;
        
        public ICommand GetWordsCommand
        {
            get
            {
                if (getWordsCommand == null)
                {
                    getWordsCommand = new AsyncCommand(x => true, async (param) =>
                    {
                        IsBusy = true;
                        using (var uofw = DALHelper.CreateUnitOfWork)
                        {
                            try
                            {
                                var list = await uofw.GetRepository<Word>()
                                .All()
                                .ToListAsync();


                                Words = new ObservableCollection<Word>(list);
                            }
                            catch(Exception ex)
                            {
                                var a = MessageBox.Show(ex.ToString(), "Can't get data");
                            }
                        }
                        IsBusy = false;
                    });
                }
                return getWordsCommand;
            }
        }

        public ICommand AddWordCommand
        {
            get
            {
                if (addWordCommand == null)
                {
                    addWordCommand = new AsyncCommand(x => true, async (param) =>
                    {
                        var gridView = param as RadGridView;
                        gridView.BeginInsert();
                    });
                }
                return addWordCommand;
            }
        }

        public ICommand DeleteWordCommand
        {
            get
            {
                if (deleteWordCommand == null)
                {
                    deleteWordCommand = new AsyncCommand(x => true, async (param) =>
                    {
                        var gridView = param as RadGridView;
                        foreach (var item in gridView.SelectedItems)
                        {
                            var word = item as Word;
                            _toSynchronize.Add(new UpdatedWordDTO(OperationType.Remove, word));
                            Words.Remove(word);
                        }
                    });
                }
                return deleteWordCommand;
            }
        }

        public ICommand ValidateCommand
        {
            get
            {
                if (validateCommand == null)
                {
                    validateCommand = new AsyncCommand(x => true, async (param) =>
                    {
                        var eventArg = param as GridViewRowValidatingEventArgs;
                        var item = eventArg.Row.Item as Word;
                        eventArg.IsValid = item.IsCorrect();
                    });
                }
                return validateCommand;
            }
        }

        public ICommand RowChangedCommand
        {
            get
            {
                if (rowChangedCommand == null)
                {
                    rowChangedCommand = new AsyncCommand(x => true, async (param) =>
                    {
                        var eventArg = param as GridViewRowEditEndedEventArgs;
                        if (eventArg == null)
                            return;
                        var data = eventArg.NewData as Word;
                        _toSynchronize.Add(new UpdatedWordDTO(
                            eventArg.EditOperationType == GridViewEditOperationType.Edit ? 
                            OperationType.Edit : 
                            OperationType.Add, 
                            data));
                    });
                }
                return rowChangedCommand;
            }
        }

        public ICommand CommitUpdateCommand
        {
            get
            {
                if (commitUpdateCommand == null)
                {
                    commitUpdateCommand = new AsyncCommand(x => true, async (param) =>
                    {
                        IsBusy = true;
                        var success = true;
                        await Task.Delay(1000);
                        using (var uofw = DALHelper.CreateUnitOfWork)
                        {
                            var repo = uofw.GetRepository<Word>();
                            foreach (var item in _toSynchronize)
                            {
                                try
                                {
                                    if (item.OperationType == OperationType.Edit)
                                        repo.Update(item.Word);
                                    else if (item.OperationType == OperationType.Add)
                                        repo.Create(item.Word);
                                    else
                                        repo.Delete(item.Word);
                                }
                                catch(Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                    success = false;
                                }
                            }
                            try
                            {
                                await uofw.SaveChangesAsync();
                                if(success)
                                    _toSynchronize.Clear();
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        IsBusy = false;
                    });
                }
                return commitUpdateCommand;
            }
        }
    }
}
