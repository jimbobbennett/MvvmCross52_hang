using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Countr.Core.Models;
using Countr.Core.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;

namespace Countr.Core.ViewModels
{
    public class CountersViewModel : MvxViewModel
    {
        readonly ICountersService service;
        readonly IMvxNavigationService navigationService;
        readonly MvxSubscriptionToken token;

        public CountersViewModel(ICountersService service,
                                 IMvxMessenger messenger,
                                 IMvxNavigationService navigationService)
        {
            this.service = service;
            this.navigationService = navigationService;
            ShowAddNewCounterCommand = new MvxAsyncCommand(ShowAddNewCounter);
            token = messenger.SubscribeOnMainThread<CountersChangedMessage>(async m => await LoadCounters());
            Counters = new ObservableCollection<CounterViewModel>();
        }

        public ObservableCollection<CounterViewModel> Counters { get; }

        public override async Task Initialize()
        {
            await LoadCounters();
        }

        public async Task LoadCounters()
        {
            Counters.Clear();

            foreach (var counter in await service.GetAllCounters())
            {
                var viewModel = new CounterViewModel(service, navigationService);
                viewModel.Prepare(counter);
                Counters.Add(viewModel);
            }
        }

        public IMvxAsyncCommand ShowAddNewCounterCommand { get; }

        async Task ShowAddNewCounter()
        {
            await navigationService.Navigate<CounterViewModel, Counter>(new Counter());
        }
    }
}