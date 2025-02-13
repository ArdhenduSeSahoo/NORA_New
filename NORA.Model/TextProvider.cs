using Damco.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public partial class TextProvider { public virtual AutomaticTransporterSelectionTextProvider AutomaticTransporterSelection { get { return GetProvider<AutomaticTransporterSelectionTextProvider>(); } } }
    public class AutomaticTransporterSelectionTextProvider : MessageTextProvider
    {
        public string Overruled { get { return GetText("Automatic transporter selection for this transport was overruled previously"); } }
        public string CustomerRequestedSpecificTransporter { get { return GetText("The customer requested a specific transporter"); } }
        public string NotSupportedForMultipleCustomerTransports { get { return GetText("Automatic transporter selection is not supported for multi-customer transports"); } }
        public string CustomerDoesNotHaveSetup { get { return GetText("This customer does not have a setup for automatic transporter selection"); } }
        public string NoRatesFoundForRemainingTransporters { get { return GetText("No rates found for remaining transporters"); } }
        public string NoLeadTimesFoundForRemainingTransporters { get { return GetText("No lead times found for remaining transporters"); } }
        public string NoPerformancesFoundForRemainingTransporters { get { return GetText("No performances found for remaining transporters"); } }
        public string NoAllocationsFoundForRemainingTransporters { get { return GetText("No allocations found for remaining transporters"); } }

        public string TransporterRates(IEnumerable<Tuple<int, decimal>> rates) { return GetTextForList("Rate[s]:\n{0.Item1}: {0.Item2}\n{0.Item1}: {0.Item2}", "No rates found", rates); }

        public string TransporterLeadTimes(IEnumerable<Tuple<int, int>> rates) { return GetTextForList("Lead time[s]:\n{0.Item1}: {0.Item2}\n{0.Item1}: {0.Item2}", "No lead times found", rates); }
        public string TransporterPerformances(IEnumerable<Tuple<int, int>> rates) { return GetTextForList("Performance:\n{0.Item1}: {0.Item2}\n{0.Item1}: {0.Item2}", "No performance data found", rates); }
        public string TransporterAllocations(IEnumerable<Tuple<int, decimal>> rates) { return GetTextForList("Allocation[s]:\n{0.Item1}: {0.Item2}\n{0.Item1}: {0.Item2}", "No allocations found", rates); }
        public string TransporterRanks(IEnumerable<Tuple<int, decimal, string>> rates) { return GetTextForList("Rank[s]:\n{0.Item1}: {0.Item2} = {0.Item3}\n{0.Item1}: {0.Item2} - {0.Item3}", "No transporters to rank", rates); }

        public string Selected(int transporterId) { return GetText("Selected {0}", Params(transporterId)); }
        public string PickUpZoneNotFound { get { return GetText("Pick-up zone not found"); } }
        public string DeliveryZoneNotFound { get { return GetText("Delivery zone not found"); } }
        public string BaseDate(DateTime date) { return GetText("Base date: {0}", Params(date)); }
        public string CalendarType(int? calendarTypeId) { return GetText("Calendar type: {0}", Params(calendarTypeId)); }

    }

}

