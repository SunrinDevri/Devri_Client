using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace Devri.Common
{
    class Timer
    {
        TimeSpan delay = TimeSpan.FromMinutes(3);

        ThreadPoolTimer DelayTimer = ThreadPoolTimer.CreateTimer(
            (source) =>
            {
                //
                // TODO: Work
                //

                //
                // Update the UI thread by using the UI core dispatcher.
                //
                System.ServiceModel.Dispatcher.RunAsync(
                    CoreDispatcherPriority.High,
                    () =>
                    {
                //
                // UI components can be accessed within this scope.
                //

            });

            }, delay);
    }
}
