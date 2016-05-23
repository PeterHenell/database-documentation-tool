using ExtendedPropertiesDocumentationTool;
using PeterHenell.SSMS.Plugins.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ExtendedPropertiesPlugin
{
    public class ExtendedPropertiesPlugin : PeterHenell.SSMS.Plugins.Plugins.CommandPluginBase
    {
        public ExtendedPropertiesPlugin() :
            base("ExtendedProperties", "Documentation", "Extended Properties", "global::Ctrl+Alt+P")
        {

        }


        public override void ExecuteCommand(System.Threading.CancellationToken token)
        {
            var connectionString = ConnectionManager.GetConnectionStringForCurrentWindow();

            var thread = new Thread( (a) =>
            {
                var bw = new MainWindow();
                bw.Show();
                bw.Closed += (s, e) => bw.Dispatcher.InvokeShutdown();
                Dispatcher.Run();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
