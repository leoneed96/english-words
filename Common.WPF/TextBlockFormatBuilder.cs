using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Common.WPF
{
    public class TextBlockFormatBuilder
    {
        private static string _indent = "IND";
        private static SolidColorBrush _transparentBrush = new SolidColorBrush(Colors.Transparent);
        private static SolidColorBrush _blackBrush = new SolidColorBrush(Colors.Black);

        private ObservableCollection<Inline> Inlines { get; set; } = new ObservableCollection<Inline>();
        public TextBlockFormatBuilder Clear()
        {
            this.Inlines = new ObservableCollection<Inline>();
            return this;
        }
        public ObservableCollection<Inline> GetInlines() => Inlines;
        public TextBlockFormatBuilder Indent()
        {
            var indent = new Run(_indent) { Foreground = _transparentBrush };
            this.Inlines.Add(indent);
            return this;
        }
        public TextBlockFormatBuilder IndentedText(string str)
        {
            Break();
            Indent();
            this.Inlines.Add(new Run(str) {Foreground = _blackBrush });
            return this;
        }
        public TextBlockFormatBuilder Text(string str)
        {
            this.Inlines.Add(new Run(str));
            return this;
        }
        public TextBlockFormatBuilder Cursive(string str)
        {
            this.Inlines.Add(new Run(str) { FontStyle = FontStyles.Italic });
            return this;
        }
        public TextBlockFormatBuilder Bold(string str)
        {
            this.Inlines.Add(new Run(str) { FontWeight = FontWeights.Bold });
            return this;
        }
        public TextBlockFormatBuilder ListItem(string str, 
            string listItemBeginsWith = "-", 
            string listItemEndsWith = ",")
        {
            Break();
            Indent();
            this.Inlines.Add(new Run(listItemBeginsWith + " " + str + "" + listItemEndsWith));
            return this;
        }
        public TextBlockFormatBuilder Break()
        {
            this.Inlines.Add(new LineBreak());
            return this;
        }
    }
}
