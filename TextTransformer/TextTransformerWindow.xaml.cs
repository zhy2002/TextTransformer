using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

using TextTransformer.Logic;
using TextTransformer.UserControls;
using ZCL.RTScript;
using ZCL.RTScript.Logic.Expression.Literal;


namespace TextTransformer
{
    /// <summary>
    /// Handel events for MainWindow.xaml.
    /// </summary>
    public partial class TextTransformerWindow : Window
    { 
        public TextTransformerWindow()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            InitializeComponent();
        }

        /// <summary>
        /// each get call creates a new instance that represents the state of the UI.
        /// </summary>
        RuleSet CurrentRuleSet
        {
            get
            {
                bool error = false;
                RuleSet rs = new RuleSet();
                foreach (ListViewItem item in RuleList.Items)
                {
                    RuleBox rb = item.Content as RuleBox;
                    var matchText = rb.FindName("matchText") as TextBox;
                    try
                    {
                        rs.AddRule(rb.RuleData);
                        matchText.Style = Resources["commontext"] as Style;
                    }
                    catch (ReplaceException)
                    {
                        matchText.Style = Resources["errortext"] as Style;
                        matchText.GotFocus += ruleBoxField_GotFocus;
                        error = true;
                        break;
                    }
                }
                return error ? null : rs;
            }

            set
            {
                RuleList.Items.Clear(); //update UI
                int count = value.Count;
                for (int i = 0; i < count; i++)
                {
                    AppendRule(value.At(i));
                }
            }
        }

        /// <summary>
        /// Append a rule in the list view.
        /// </summary>
        /// <param name="rule"></param>
        private void AppendRule(RTRuleData rule)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            lvi.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
            var ruleui = new RuleBox();
            ruleui.RuleData = rule;
            lvi.Content = ruleui;
            var ruleCollection = RuleList.Items;
            int count = ruleCollection.Count;
            ruleui.Index = count;
            ruleCollection.Insert(count, lvi);
            RuleList.SelectedIndex = count;
        }

        /// <summary>
        /// restore style back to normal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ruleBoxField_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt1 = sender as TextBox;
            txt1.Style = Resources["commontext"] as Style;
            txt1.GotFocus -= ruleBoxField_GotFocus;
        }

        private void AddRuleCmdBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void AddRuleCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AppendRule(null);
            e.Handled = true;
        }

        private void DeleteRuleCmdBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RuleList.SelectedItem != null;
            e.Handled = true;
        }

        private void DeleteRuleCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedIndex = RuleList.SelectedIndex;
            RuleList.Items.Remove(RuleList.SelectedItem);
            for (int i = selectedIndex; i < RuleList.Items.Count; i++)//renumber subsequent items
            {
                ((RuleBox)((ListViewItem)RuleList.Items[i]).Content).Index = i;
            }

            selectedIndex--;//select the previous one
            if (selectedIndex < 0)
                selectedIndex = 0;

            if (selectedIndex < RuleList.Items.Count)
            {
                RuleList.SelectedIndex = selectedIndex;
            }
            RuleList.Focus();
            e.Handled = true;
        }

        private void RunCmdBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (srcbox.Text.Length == 0 || RuleList.Items.Count == 0)
            {
                e.CanExecute = false;
                e.Handled = true;
                return;
            }

            e.CanExecute = true;
            e.Handled = true;
        }

        private void RunCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var ruleset = CurrentRuleSet;
            if (ruleset == null)
            {
                MessageBox.Show("Please correct the error before continue.", "Input Error");
                return;
            }

            try
            {
                outputbox.Text = ruleset.Run(srcbox.Text);
                var text = outputbox.Text ?? String.Empty;
                txtLineCount.Text = "Line Count: " + text.Split('\n').Length.ToString() + ", Char Count: " + text.Length;
            }
            catch (ReplaceException ex)
            {
                MessageBox.Show(ex.Message);

                int sourceIndex = ex.SourceIndex;
                int ruleId = ex.RuleId;

                if (ruleId != -1 && sourceIndex != -1)
                {
                    ruleId--;
                    RuleBox rb = (RuleList.Items[ruleId] as ListViewItem).Content as RuleBox;
                    var sbox = rb.FindName("replaceText") as TextBox;
                    sbox.Focus();
                    sbox.Select(sourceIndex, 1);
                }
            }
            catch (RTParsingException ex2) {
                MessageBox.Show(ex2.Message);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender == miLoad)
            {
                System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.Filter = "Rule Set Files (*.rs)|*.rs";
                dialog.DefaultExt = "rs";
                dialog.InitialDirectory = Directory.GetCurrentDirectory();
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    RuleSet rset = FileUtil.RestoreFrom<RuleSet>(dialog.FileName);
                    if (rset == null)
                    {
                        MessageBox.Show("Unrecognized file format.");
                        return;
                    }
                    CurrentRuleSet = rset;
                    Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(dialog.FileName));
                }

            }
            else if (sender == miSave)
            {
                System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
                dialog.Filter = "Rule Set Files (*.rs)|*.rs";
                dialog.DefaultExt = "rs";
                dialog.InitialDirectory = Directory.GetCurrentDirectory();
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    FileUtil.SaveTo(CurrentRuleSet, dialog.FileName);
                    Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(dialog.FileName));
                }
            }
        }

        private void ctxMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //if no rule is under cursor
            if (RuleList.SelectedItem == null)
            {
                foreach (FrameworkElement it in ctxMenu.Items)
                {
                    if (it == miLoad)
                    {
                        it.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        it.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }

                return;
            }

            foreach (FrameworkElement it in ctxMenu.Items)
            {
                it.Visibility = System.Windows.Visibility.Visible;
            }
            //load rule settings onto the context menu
            RuleBox rb = (RuleList.SelectedItem as ListViewItem).Content as RuleBox;
            RegexOptions options = rb.RegOptions;
            RegexOptions temp;
            foreach (FrameworkElement it in ctxMenu.Items)
            {
                if (it == miMatchType)
                {
                    cbMatchType.SelectedItem = rb.MatchType;
                }
                else
                {
                    MenuItem item = it as MenuItem;
                    if (item == null) continue;
                    string optiontext = item.Tag as string;
                    if (optiontext == null || !item.IsCheckable) continue;
                    if (!Enum.TryParse<RegexOptions>(optiontext, out temp)) continue;
                    item.IsChecked = (options & temp) != RegexOptions.None;
                }
            }
        }

        private void ctxMenu_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            if (RuleList.SelectedItem == null)
            {
                return;
            }

            //copy settings back to the selected rule
            RuleBox rb = (RuleList.SelectedItem as ListViewItem).Content as RuleBox;
            RegexOptions options = RegexOptions.None;
            RegexOptions temp;
            foreach (var it in ctxMenu.Items)
            {
                if (it == miMatchType)
                {
                    rb.MatchType = (MatchType)cbMatchType.SelectedItem;
                }
                else
                {
                    MenuItem item = it as MenuItem;
                    if (item == null) continue;
                    string optiontext = item.Tag as string;
                    if (optiontext == null) continue;
                    if (item.IsCheckable && item.IsChecked && Enum.TryParse<RegexOptions>(optiontext, out temp)) options |= temp;
                }
            }
            rb.RegOptions = options;
        }

        private void hlTut_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void hlHelp_Click(object sender, RoutedEventArgs e)
        {
            var helpWindow = new HelpWindow();
            helpWindow.Owner = this;
            helpWindow.ShowDialog();
        }

        private void cbMatchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ctxMenu.IsOpen)
            {
                ctxMenu.IsOpen = false;
            }
        }

        private void srcbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int row = srcbox.GetLineIndexFromCharacterIndex(srcbox.CaretIndex);
            int col = srcbox.CaretIndex - srcbox.GetCharacterIndexFromLineIndex(row);
            lblStatus.Text = "Line " + (row + 1) + ", Char " + (col + 1);
        }
    }


}
