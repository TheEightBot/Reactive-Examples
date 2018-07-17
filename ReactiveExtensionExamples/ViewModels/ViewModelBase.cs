using System;
using ReactiveUI;
namespace ReactiveExtensionExamples.ViewModels
{
    public class ViewModelBase : ReactiveObject, ISupportsActivation
    {
        protected readonly ViewModelActivator viewModelActivator = new ViewModelActivator();
        public ViewModelActivator Activator
        {
            get { return viewModelActivator; }
        }
    }
}
