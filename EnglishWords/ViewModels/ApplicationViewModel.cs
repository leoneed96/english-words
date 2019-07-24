using Common.WPF.Commands;
using EnglishWords.DAL;
using EnglishWords.Data.Entities;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using ViewModelBase = Common.WPF.ViewModels.ViewModelBase;

namespace EnglishWords.ViewModels
{
    public class ApplicationViewModel : ViewModelBase
    {
        private NotificationManager notificationManager;
        private DispatcherTimer DispatcherTimer;
        private IList<Word> _cache = new List<Word>();
        private Word _lastShown;
        private bool _ignoreLast;
        private ICommand startShowCommand;
        public ApplicationViewModel()
        {
            WordsViewModel = new WordsViewModel();
            SettingsViewModel = new SettingsViewModel();
            notificationManager = new NotificationManager();
            SettingsViewModel.OnIntervalChanged += SettingsViewModel_OnIntervalChanged;
            SettingsViewModel.OnOrderChanged += (a,b) => ResetCache();
            DispatcherTimer = new DispatcherTimer();
            DispatcherTimer.Tick += DispatcherTimer_Tick;
            //WordsViewModel.GetWordsCommand.Execute(null);
        }

        private async void DispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            await Task.Delay(2000);
            if (_cache == null || !_cache.Any())
                await RefreshCache();

            var word = _cache.First();
            notificationManager.Show(new NotificationContent()
            {
                Title = word.English,
                Type = NotificationType.Information,
                Message = $"{word.Russian} - {word.Description}"
            },
            expirationTime: TimeSpan.FromSeconds(SettingsViewModel.HideInterval));
            
            _lastShown = word;
            _cache.Remove(word);
        }
        private void ResetCache()
        {
            _cache.Clear();
            _ignoreLast = false;
            _lastShown = null;
        }
        public void StopTimer() { DispatcherTimer.Tick -= DispatcherTimer_Tick;  DispatcherTimer.Stop(); }
        private async Task RefreshCache()
        {
            using (var uofw = DALHelper.CreateUnitOfWork)
            {
                var query = uofw.GetRepository<Word>().All();
                switch (SettingsViewModel.ShowOrder)
                {
                    case ShowOrder.AddDateAsc:
                        {
                            query = query.OrderBy(x => x.ID);
                            if (_lastShown != null && !_ignoreLast)
                                query = query.Where(x => x.ID > _lastShown.ID);
                            break;
                        }
                    case ShowOrder.AddDateDesc:
                        {
                            query = query.OrderByDescending(x => x.ID);
                            if (_lastShown != null && !_ignoreLast)
                                query = query.Where(x => x.ID < _lastShown.ID);
                            break;
                        }
                    case ShowOrder.Random:
                        {
                            var seed = (int)DateTime.Now.Ticks % 9395713;//a random number
                            query = query
                               .OrderBy(item => SqlFunctions.Rand(item.ID * seed % 577317));
                            break;
                        }
                    default:
                        break;
                }
                _cache = await query.Take(20).ToListAsync();
                if (await query.CountAsync() <= 20)
                    _ignoreLast = true;
            }
        }
        private void SettingsViewModel_OnIntervalChanged(object sender, System.EventArgs e)
        {
            DispatcherTimer.Interval = TimeSpan.FromSeconds(SettingsViewModel.ShowInterval);
        }

        public WordsViewModel WordsViewModel { get; private set; }
        public SettingsViewModel SettingsViewModel { get; private set; }
        public ICommand StartShowCommand
        {
            get
            {
                if (startShowCommand == null)
                {
                    startShowCommand = new AsyncCommand(x => true, async (param) =>
                    {
                        if (!DispatcherTimer.IsEnabled)
                        {
                            DispatcherTimer.Interval = TimeSpan.FromSeconds(SettingsViewModel.ShowInterval);
                            DispatcherTimer.Start();
                        }
                        else
                        {
                            ResetCache();
                            DispatcherTimer.Stop();
                            notificationManager.Show(new NotificationContent()
                            {
                                Title = "Info",
                                Type = NotificationType.Warning,
                                Message = "Timer stopped"
                            });
                        }
                    });
                }
                return startShowCommand;
            }
        }
    }
}
