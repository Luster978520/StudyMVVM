using StudyMVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace StudyMVVM.Views
{
    /// <summary>
    /// The module router builds a different, functional native WPF view for every lesson.
    /// Code-behind here owns visual experiments only; course navigation stays in the view model.
    /// All parsed XAML is application-owned, never arbitrary user input.
    /// </summary>
    public sealed class TemplateLessonWorkspace : UserControl
    {
        public static readonly DependencyProperty LessonProperty = DependencyProperty.Register(
            "Lesson", typeof(TemplateLesson), typeof(TemplateLessonWorkspace), new PropertyMetadata(null, OnLessonChanged));
        public TemplateLesson Lesson { get => (TemplateLesson)GetValue(LessonProperty); set => SetValue(LessonProperty, value); }
        private StackPanel experiment;
        private TextBox source;
        private TextBlock result;
        private const string Xmlns = " xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'";
        private const string BasicBody = "<Border x:Name='Chrome' CornerRadius='12' Background='{TemplateBinding Background}' BorderBrush='{TemplateBinding BorderBrush}' BorderThickness='{TemplateBinding BorderThickness}'><ContentPresenter Margin='{TemplateBinding Padding}' HorizontalAlignment='Center' VerticalAlignment='Center'/></Border>";
        public string ModuleNumber => Lesson?.Number;
        public string CurrentCode => source?.Text;
        public string LastResult => result?.Text;

        public TemplateLessonWorkspace()
        {
            Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/StudyMVVM;component/Resource/TemplateLabStyles.xaml", UriKind.Relative) });
        }
        private static void OnLessonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TemplateLessonWorkspace)d).Build();
        }
        private T Identify<T>(T control, string name) where T : DependencyObject
        {
            AutomationProperties.SetAutomationId(control, "Module" + Lesson.Number + "/" + name);
            return control;
        }
        private static Brush Brush(string hex) => (Brush)new BrushConverter().ConvertFromInvariantString(hex);
        private TextBlock Text(string text, double size = 14, string color = "#282637") => new TextBlock
        { Text = text, FontSize = size, Foreground = Brush(color), TextWrapping = TextWrapping.Wrap, LineHeight = size * 1.7, Margin = new Thickness(0, 0, 0, 12) };
        private Border Card(UIElement content, bool dark = false) => new Border
        { Child = content, Background = Brush(dark ? "#1D1E30" : "#FFFFFF"), CornerRadius = new CornerRadius(12), Padding = new Thickness(24) };
        private Button Action(string text, Action action, string id = null)
        {
            var button = new Button { Content = text, Style = (Style)FindResource("LabSecondary"), Margin = new Thickness(0, 0, 10, 10) };
            if (id != null) Identify(button, id);
            button.Click += (s, e) => action();
            return button;
        }
        private WrapPanel Actions(params UIElement[] items)
        {
            var row = new WrapPanel { Margin = new Thickness(0, 4, 0, 10) };
            foreach (var item in items) row.Children.Add(item);
            experiment.Children.Add(row);
            return row;
        }
        private ComboBox Choices(string title, string id, params string[] items)
        {
            experiment.Children.Add(Text(title, 12, "#777486"));
            var combo = Identify(new ComboBox { ItemsSource = items, SelectedIndex = 0, MinHeight = 34, Margin = new Thickness(0, 0, 0, 18) }, id);
            experiment.Children.Add(combo);
            return combo;
        }
        private CheckBox Check(string text, string id, bool enabled = true)
        {
            var check = Identify(new CheckBox { Content = text, IsChecked = enabled, Margin = new Thickness(0, 0, 0, 18) }, id);
            experiment.Children.Add(check);
            return check;
        }
        private Slider Range(string title, string id, double min, double max, double value)
        {
            experiment.Children.Add(Text(title, 12, "#777486"));
            var slider = Identify(new Slider { Minimum = min, Maximum = max, Value = value, TickFrequency = 1, IsSnapToTickEnabled = true, Margin = new Thickness(0, 0, 0, 20) }, id);
            experiment.Children.Add(slider);
            return slider;
        }
        private Border Stage(UIElement child, double height = 170)
        {
            var stage = new Border { Child = child, MinHeight = height, Padding = new Thickness(20), Background = Brush("#F7F7FB"), CornerRadius = new CornerRadius(12), Margin = new Thickness(0, 0, 0, 18) };
            experiment.Children.Add(stage);
            return stage;
        }
        private Button Preview(string text, string id = "Preview")
        {
            return Identify(new Button { Content = text, Background = Brush("#7055D5"), Foreground = Brushes.White,
                Padding = new Thickness(24, 14, 24, 14), HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center, MinWidth = 170, Template = CreateTemplate(BasicBody) }, id);
        }
        private static ControlTemplate CreateTemplate(string body, string target = "Button") => (ControlTemplate)XamlReader.Parse("<ControlTemplate" + Xmlns + " TargetType='" + target + "'>" + body + "</ControlTemplate>");
        private static T Parse<T>(string markup) => (T)XamlReader.Parse(markup);
        private void Code(string code, string message)
        {
            source.Text = code;
            result.Text = message;
        }
        private void Build()
        {
            if (Lesson == null) { Content = null; return; }
            var root = new StackPanel();
            var intro = new StackPanel();
            intro.Children.Add(Text(Lesson.Topics, 17, "#7055D5"));
            intro.Children.Add(Text(Lesson.Explanation, 13, "#777486"));
            root.Children.Add(Card(intro));
            var split = new Grid { Margin = new Thickness(0, 20, 0, 20) };
            split.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.15, GridUnitType.Star) });
            split.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });
            split.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            experiment = new StackPanel();
            experiment.Children.Add(Text("交互实验 / " + Lesson.Title, 18));
            split.Children.Add(Card(experiment));
            var codePanel = new StackPanel();
            codePanel.Children.Add(Text("本模块代码 · 随操作更新", 14, "#DCD9F0"));
            source = Identify(new TextBox { IsReadOnly = true, AcceptsReturn = true, FontFamily = new FontFamily("Consolas"), FontSize = 13,
                Foreground = Brush("#DCD9F0"), Background = Brushes.Transparent, BorderThickness = new Thickness(0),
                TextWrapping = TextWrapping.Wrap, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, MinHeight = 310, MaxHeight = 530 }, "Code");
            codePanel.Children.Add(source);
            var copy = Action("复制本课代码", () => { try { Clipboard.SetText(source.Text); result.Text = "本课代码已复制。"; } catch (System.Runtime.InteropServices.COMException) { result.Text = "剪贴板正在使用中，请稍后重试。"; } }, "Copy");
            codePanel.Children.Add(copy);
            var codeCard = Card(codePanel, true); Grid.SetColumn(codeCard, 2); split.Children.Add(codeCard);
            root.Children.Add(split);
            result = Identify(Text("", 13, "#7055D5"), "Result");
            root.Children.Add(result);
            var builders = new Dictionary<string, Action> { {"01", Basics}, {"02", FirstTemplate}, {"03", BindingExperiment},
                {"04", BuildTriggers}, {"05", Motion}, {"06", Namescope}, {"07", Parts}, {"08", Items}, {"09", Themes},
                {"10", Gallery}, {"11", Counter}, {"12", Diagnostics} };
            Action build;
            if (builders.TryGetValue(Lesson.Number, out build)) build();
            else throw new InvalidOperationException("Unknown lesson: " + Lesson.Number);
            AddKnowledgeCheck(root);
            Content = root;
        }

        private void Basics()
        {
            var mode = Choices("比较修改属性与替换视觉结构", "Mode", "默认 Button", "Style：修改属性", "ControlTemplate：替换结构");
            var button = Identify(new Button { Content = "我仍然是一个 Button", Width = 220, Height = 90, HorizontalAlignment = HorizontalAlignment.Center }, "Preview");
            Stage(button); int clicks = 0;
            button.Click += (s, e) => result.Text = "Click 已触发 " + (++clicks) + " 次。外观改变不会删除 Button 的点击行为。";
            Action update = () =>
            {
                button.ClearValue(Control.TemplateProperty); button.ClearValue(FrameworkElement.StyleProperty);
                if (mode.SelectedIndex == 1)
                {
                    var style = new Style(typeof(Button)); style.Setters.Add(new Setter(Control.BackgroundProperty, Brush("#7055D5")));
                    style.Setters.Add(new Setter(Control.ForegroundProperty, Brushes.White)); style.Setters.Add(new Setter(Control.FontSizeProperty, 18.0)); button.Style = style;
                }
                if (mode.SelectedIndex == 2)
                {
                    button.Background = Brush("#7055D5"); button.Foreground = Brushes.White;
                    button.Template = CreateTemplate("<Grid><Ellipse Fill='{TemplateBinding Background}'/><ContentPresenter HorizontalAlignment='Center' VerticalAlignment='Center'/></Grid>");
                }
                else { button.ClearValue(Control.BackgroundProperty); button.ClearValue(Control.ForegroundProperty); }
                Code(mode.SelectedIndex == 2 ? "<ControlTemplate TargetType=\"Button\">\n  <Grid>\n    <Ellipse Fill=\"{TemplateBinding Background}\"/>\n    <ContentPresenter HorizontalAlignment=\"Center\"\n                      VerticalAlignment=\"Center\"/>\n  </Grid>\n</ControlTemplate>" : mode.SelectedIndex == 1 ? "<Style TargetType=\"Button\">\n  <Setter Property=\"Background\" Value=\"#7055D5\"/>\n  <Setter Property=\"Foreground\" Value=\"White\"/>\n  <Setter Property=\"FontSize\" Value=\"18\"/>\n</Style>" : "<Button Content=\"我仍然是一个 Button\"/>\n<!-- 使用系统默认模板 -->", "当前：" + mode.SelectedItem + "。点击预览按钮验证 Click 行为。");
            };
            mode.SelectionChanged += (s, e) => update(); update();
        }

        private void FirstTemplate()
        {
            var padding = Range("内容 Padding（0–36）", "Padding", 0, 36, 12);
            var align = Choices("HorizontalContentAlignment", "Alignment", "Center", "Left", "Right");
            var show = Check("保留 ContentPresenter 内容宿主", "ContentHost");
            var button = Preview("图标 ★ + 可复用内容"); button.Width = 300; button.MinHeight = 80; Stage(button);
            Action update = () =>
            {
                string alignment = (string)align.SelectedItem;
                string body = "<Border Background='{TemplateBinding Background}' CornerRadius='12'>" + (show.IsChecked == true ? "<ContentPresenter Margin='{TemplateBinding Padding}' HorizontalAlignment='{TemplateBinding HorizontalContentAlignment}' VerticalAlignment='Center'/>" : "") + "</Border>";
                button.Padding = new Thickness(padding.Value); button.HorizontalContentAlignment = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), alignment); button.Template = CreateTemplate(body);
                Code("<Button Padding=\"" + padding.Value + "\"\n        HorizontalContentAlignment=\"" + alignment + "\">\n  <Button.Template>\n    <ControlTemplate TargetType=\"Button\">\n" + body.Replace("><", ">\n<") + "\n    </ControlTemplate>\n  </Button.Template>\n</Button>", show.IsChecked == true ? "ContentPresenter 已保留，内容间距与对齐来自外部 Button。" : "删除内容宿主后，按钮依然存在，但文字不会显示。");
            };
            padding.ValueChanged += (s, e) => update(); align.SelectionChanged += (s, e) => update(); show.Checked += (s, e) => update(); show.Unchecked += (s, e) => update(); update();
        }

        private void BindingExperiment()
        {
            var mode = Choices("选择模板内部 TextBox.Text 的连接方式", "BindingMode", "TemplateBinding（单向）", "Binding + TemplatedParent（OneWay）", "Binding + TemplatedParent（TwoWay）");
            experiment.Children.Add(Text("外部数据源：修改这里，观察模板内部", 12));
            var input = Identify(new TextBox { Text = "Hello, Template", Padding = new Thickness(10), Margin = new Thickness(0, 0, 0, 18) }, "Source"); experiment.Children.Add(input);
            var button = Preview(""); button.Width = 300; button.SetBinding(ContentControl.ContentProperty, new Binding("Text") { Source = input, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }); Stage(button);
            Action update = () =>
            {
                string expression = mode.SelectedIndex == 0 ? "{TemplateBinding Content}" : "{Binding Content, RelativeSource={RelativeSource TemplatedParent}, Mode=" + (mode.SelectedIndex == 2 ? "TwoWay" : "OneWay") + ", UpdateSourceTrigger=PropertyChanged}";
                button.Template = CreateTemplate("<Border Background='#F0ECFC' Padding='16' CornerRadius='12'><TextBox x:Name='InnerEditor' Text='" + expression + "' Padding='8' Foreground='#282637'/></Border>");
                Code("<!-- 外部数据源 → Button.Content → 模板内部 -->\n<ControlTemplate TargetType=\"Button\">\n  <TextBox Text=\"" + expression + "\"/>\n</ControlTemplate>", "在预览框中输入文字。只有 TwoWay 模式会将内部编辑写回外部数据源。");
            };
            mode.SelectionChanged += (s, e) => update(); update();
        }

        private void BuildTriggers()
        {
            var enabled = Check("IsEnabled：允许操作", "Enabled");
            var button = Identify(new ToggleButton { Content = "点击切换 IsChecked", Width = 240, Height = 68, Background = Brush("#7055D5"), Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center }, "Preview");
            const string triggers = "<ControlTemplate.Triggers><Trigger Property='IsMouseOver' Value='True'><Setter TargetName='Chrome' Property='Background' Value='#5940B8'/></Trigger><Trigger Property='IsChecked' Value='True'><Setter TargetName='Chrome' Property='Background' Value='#208261'/></Trigger><MultiTrigger><MultiTrigger.Conditions><Condition Property='IsChecked' Value='True'/><Condition Property='IsMouseOver' Value='True'/></MultiTrigger.Conditions><Setter TargetName='Chrome' Property='Background' Value='#145C46'/></MultiTrigger><Trigger Property='IsEnabled' Value='False'><Setter TargetName='Chrome' Property='Background' Value='#CDC8DF'/></Trigger></ControlTemplate.Triggers>";
            button.Template = CreateTemplate(BasicBody + triggers, "ToggleButton"); Stage(button);
            Action state = () => result.Text = "IsEnabled=" + button.IsEnabled + "  IsChecked=" + button.IsChecked + "  IsMouseOver=" + button.IsMouseOver + "。选中与悬停同时成立时命中 MultiTrigger。";
            enabled.Checked += (s, e) => { button.IsEnabled = true; state(); }; enabled.Unchecked += (s, e) => { button.IsEnabled = false; state(); };
            button.Checked += (s, e) => state(); button.Unchecked += (s, e) => state(); button.MouseEnter += (s, e) => state(); button.MouseLeave += (s, e) => state();
            Code(triggers.Replace("><", ">\n<"), "悬停、点击选中、禁用，分别观察 Trigger 和 MultiTrigger 的覆盖结果。");
        }

        private void Motion()
        {
            var duration = Range("VisualTransition 过渡时长（毫秒）", "Duration", 0, 1200, 350);
            var target = Parse<Border>("<Border" + Xmlns + " x:Name='MotionRoot' Width='220' Height='100' Background='#7055D5' CornerRadius='18' RenderTransformOrigin='0.5,0.5'><Border.RenderTransform><ScaleTransform x:Name='Scale'/></Border.RenderTransform><VisualStateManager.VisualStateGroups><VisualStateGroup x:Name='DemoStates'><VisualState x:Name='Rest'><Storyboard><DoubleAnimation Storyboard.TargetName='Scale' Storyboard.TargetProperty='ScaleX' To='1' Duration='0'/><DoubleAnimation Storyboard.TargetName='Scale' Storyboard.TargetProperty='ScaleY' To='1' Duration='0'/><ColorAnimation Storyboard.TargetName='MotionRoot' Storyboard.TargetProperty='(Border.Background).(SolidColorBrush.Color)' To='#7055D5' Duration='0'/></Storyboard></VisualState><VisualState x:Name='Expanded'><Storyboard><DoubleAnimation Storyboard.TargetName='Scale' Storyboard.TargetProperty='ScaleX' To='1.16' Duration='0'/><DoubleAnimation Storyboard.TargetName='Scale' Storyboard.TargetProperty='ScaleY' To='1.16' Duration='0'/><ColorAnimation Storyboard.TargetName='MotionRoot' Storyboard.TargetProperty='(Border.Background).(SolidColorBrush.Color)' To='#5940B8' Duration='0'/></Storyboard></VisualState><VisualState x:Name='Done'><Storyboard><DoubleAnimation Storyboard.TargetName='Scale' Storyboard.TargetProperty='ScaleX' To='1' Duration='0'/><DoubleAnimation Storyboard.TargetName='Scale' Storyboard.TargetProperty='ScaleY' To='1' Duration='0'/><ColorAnimation Storyboard.TargetName='MotionRoot' Storyboard.TargetProperty='(Border.Background).(SolidColorBrush.Color)' To='#208261' Duration='0'/></Storyboard></VisualState></VisualStateGroup></VisualStateManager.VisualStateGroups><TextBlock Text='VisualStateManager' Foreground='White' HorizontalAlignment='Center' VerticalAlignment='Center'/></Border>");
            Stage(target, 210); var group = (VisualStateGroup)VisualStateManager.GetVisualStateGroups(target)[0];
            Action<string> go = state =>
            {
                group.Transitions.Clear(); group.Transitions.Add(new VisualTransition { GeneratedDuration = new Duration(TimeSpan.FromMilliseconds(duration.Value)) });
                bool found = VisualStateManager.GoToElementState(target, state, true);
                Code("<VisualStateGroup x:Name=\"DemoStates\">\n  <VisualStateGroup.Transitions>\n    <VisualTransition GeneratedDuration=\"0:0:" + (duration.Value / 1000).ToString("0.###", CultureInfo.InvariantCulture) + "\"/>\n  </VisualStateGroup.Transitions>\n  <!-- Rest / Expanded / Done 各自声明 Storyboard -->\n</VisualStateGroup>\n\nVisualStateManager.GoToElementState(root,\n    \"" + state + "\", true);", found ? "当前视觉状态：" + state + "。状态由真实的 VisualStateManager 驱动。" : "视觉树尚未连接，请加载后再切换。");
            };
            Actions(Action("Rest · 默认", () => go("Rest"), "Rest"), Action("Expanded · 放大", () => go("Expanded"), "Expanded"), Action("Done · 完成", () => go("Done"), "Done"));
            Code("// 点击状态按钮，观察颜色与缩放过渡。\n// GoToElementState 用于元素上的状态组；\n// 控件模板状态通常用 GoToState(control, ...)。", "本课演示 VisualStateGroup、VisualTransition 与 Storyboard，而非属性 Trigger。");
        }

        private void Namescope()
        {
            var a = Preview("模板实例 A", "InstanceA"); var b = Preview("模板实例 B", "InstanceB");
            var shared = CreateTemplate(BasicBody); a.Template = shared; b.Template = shared; Stage(a, 110); Stage(b, 110);
            Action<Button, string> find = (button, name) =>
            {
                a.ApplyTemplate(); b.ApplyTemplate();
                var first = (Border)a.Template.FindName("Chrome", a); var second = (Border)b.Template.FindName("Chrome", b);
                first.BorderThickness = second.BorderThickness = new Thickness(0);
                var selected = (Border)button.Template.FindName("Chrome", button); selected.BorderBrush = Brush("#208261"); selected.BorderThickness = new Thickness(4);
                Code("button" + name + ".ApplyTemplate();\nvar chrome = button" + name + ".Template\n  .FindName(\"Chrome\", button" + name + ") as Border;\n\n// A.Chrome 与 B.Chrome 是不同实例。", "已定位 " + name + ".Chrome；ReferenceEquals(A.Chrome, B.Chrome) = " + ReferenceEquals(first, second) + "。同名元素由各自模板作用域隔离。");
            };
            Actions(Action("查找 A.Chrome", () => find(a, "A"), "FindA"), Action("查找 B.Chrome", () => find(b, "B"), "FindB"));
            Code("Button A → Border:Chrome → ContentPresenter\nButton B → Border:Chrome → ContentPresenter\n\n// 同一份模板，每个实例有独立 NameScope。", "点击查找，观察哪个实例的边框被高亮。查找结果来自真实的 Template.FindName。");
        }

        private void Parts()
        {
            var keep = Check("保留 TextBox 的 PART_ContentHost 部件", "KeepPart");
            var input = Identify(new TextBox { Text = "在这里输入文字，测试文本宿主", Padding = new Thickness(12), MinHeight = 54, FontSize = 15 }, "TextInput"); Stage(input, 120);
            var slider = Identify(new Slider { Minimum = 0, Maximum = 100, Value = 40, Margin = new Thickness(0, 10, 0, 14) }, "Slider"); experiment.Children.Add(slider);
            Action inspect = () =>
            {
                input.ApplyTemplate(); slider.ApplyTemplate();
                var host = input.Template.FindName("PART_ContentHost", input); var track = slider.Template.FindName("PART_Track", slider);
                result.Text = "TextBox.PART_ContentHost：" + (host == null ? "缺失，文本无法呈现" : host.GetType().Name) + "；Slider.PART_Track：" + (track == null ? "未生成" : track.GetType().Name) + "。";
            };
            Action update = () =>
            {
                string name = keep.IsChecked == true ? "PART_ContentHost" : "WrongHostName";
                string body = "<Border Background='#F0ECFC' CornerRadius='8' Padding='{TemplateBinding Padding}'><ScrollViewer x:Name='" + name + "'/></Border>";
                input.Template = CreateTemplate(body, "TextBox"); Code("<ControlTemplate TargetType=\"TextBox\">\n" + body.Replace("><", ">\n<") + "\n</ControlTemplate>\n\n// OnApplyTemplate 中应对查找结果判空。", "切换复选框，对比部件存在与缺失时的文本呈现。"); inspect();
            };
            keep.Checked += (s, e) => update(); keep.Unchecked += (s, e) => update(); Actions(Action("检查模板部件", inspect, "Inspect")); update();
        }

        private void Items()
        {
            var display = Choices("ItemTemplate：每一项数据如何显示", "ItemTemplate", "纯文字", "卡片（标题 + 说明）");
            var layout = Choices("ItemsPanel：多项数据如何排列", "ItemsPanel", "StackPanel · 纵向", "WrapPanel · 自动换行");
            var list = Identify(new ListBox { Height = 250, BorderThickness = new Thickness(0), Background = Brushes.Transparent,
                ItemsSource = new[] { new LearningCard("ControlTemplate", "控件的视觉结构"), new LearningCard("DataTemplate", "数据的呈现方式"), new LearningCard("ItemsPanelTemplate", "集合项的布局") } }, "List");
            // A WrapPanel must be measured with a bounded width to wrap its items.
            ScrollViewer.SetHorizontalScrollBarVisibility(list, ScrollBarVisibility.Disabled);
            Stage(list, 280);
            Action update = () =>
            {
                string item = display.SelectedIndex == 0 ? "<TextBlock Text='{Binding Title}' Margin='8'/>" : "<Border Background='#F0ECFC' CornerRadius='8' Padding='14' Margin='4' Width='180'><StackPanel><TextBlock Text='{Binding Title}' Foreground='#7055D5' FontWeight='Bold'/><TextBlock Text='{Binding Detail}' Margin='0,8,0,0'/></StackPanel></Border>";
                string panel = layout.SelectedIndex == 0 ? "StackPanel" : "WrapPanel";
                list.ItemTemplate = Parse<DataTemplate>("<DataTemplate" + Xmlns + ">" + item + "</DataTemplate>"); list.ItemsPanel = Parse<ItemsPanelTemplate>("<ItemsPanelTemplate" + Xmlns + "><" + panel + "/></ItemsPanelTemplate>");
                Code("<ListBox ItemsSource=\"{Binding Lessons}\">\n  <ListBox.ItemTemplate>\n    <DataTemplate>\n" + item.Replace("><", ">\n<") + "\n    </DataTemplate>\n  </ListBox.ItemTemplate>\n  <ListBox.ItemsPanel>\n    <ItemsPanelTemplate><" + panel + "/></ItemsPanelTemplate>\n  </ListBox.ItemsPanel>\n</ListBox>", "ItemTemplate 改变数据外观，ItemsPanel 改变布局；点击条目可体验 ListBoxItem 的选择行为。");
            };
            display.SelectionChanged += (s, e) => update(); layout.SelectionChanged += (s, e) => update(); update();
        }

        private void Themes()
        {
            Resources["LessonAccent"] = Brush("#7055D5");
            var dynamicButton = Preview("DynamicResource", "Dynamic"); dynamicButton.SetResourceReference(Control.BackgroundProperty, "LessonAccent");
            var staticButton = Preview("StaticResource", "Static"); staticButton.Background = (Brush)Resources["LessonAccent"]; Stage(dynamicButton, 110); Stage(staticButton, 110);
            Action report = () => result.Text = "DynamicResource=" + dynamicButton.Background + "；StaticResource 快照=" + staticButton.Background + "。资源限定于本模块，不修改整个应用。";
            Action<string> theme = hex => { Resources["LessonAccent"] = Brush(hex); report(); };
            Actions(Action("紫色主题", () => theme("#7055D5"), "Purple"), Action("青绿主题", () => theme("#208261"), "Green"), Action("深色主题", () => theme("#282637"), "Dark"));
            Actions(Action("重新查找静态资源", () => { staticButton.Background = (Brush)Resources["LessonAccent"]; report(); }, "ResolveStatic"));
            Code("<SolidColorBrush x:Key=\"LessonAccent\" Color=\"#7055D5\"/>\n\n<Button Background=\"{StaticResource LessonAccent}\"/>\n<Button Background=\"{DynamicResource LessonAccent}\"/>\n\n// 替换资源对象，而非修改同一画刷实例。\nResources[\"LessonAccent\"] = new SolidColorBrush(...);", "点击切换主题：动态资源立即响应；静态资源保留原对象，直到重新查找。");
        }

        private void Gallery()
        {
            var choice = Choices("选择控件，操作它的关键状态", "ControlType", "Button", "ToggleButton", "CheckBox", "RadioButton", "TextBox", "ComboBox", "ListBox", "Slider", "ProgressBar");
            var enabled = Check("IsEnabled：启用当前控件", "Enabled");
            var host = new ContentControl { HorizontalContentAlignment = HorizontalAlignment.Stretch }; Stage(host, 230);
            var value = Range("Slider / ProgressBar 的 Value", "Value", 0, 100, 45);
            Control current = null;
            Action update = () =>
            {
                string name = (string)choice.SelectedItem;
                switch (name)
                {
                    case "Button": var button = Preview("Click / Command"); button.Click += (s, e) => result.Text = "Button.Click 已触发。"; current = button; break;
                    case "ToggleButton": current = new ToggleButton { Content = "点击切换 IsChecked", Height = 48 }; break;
                    case "CheckBox": current = new CheckBox { Content = "IsChecked · 三态选项", IsThreeState = true }; break;
                    case "RadioButton": var radios = new StackPanel(); radios.Children.Add(new RadioButton { Content = "选项 A", GroupName = "Lesson10", Margin = new Thickness(0, 0, 0, 12) }); radios.Children.Add(new RadioButton { Content = "选项 B", GroupName = "Lesson10" }); var container = new ContentControl { Content = radios }; current = container; break;
                    case "TextBox": current = new TextBox { Text = "输入并编辑 Text", Padding = new Thickness(12), Height = 48 }; break;
                    case "ComboBox": current = new ComboBox { ItemsSource = new[] { "默认模板", "选择与弹出层", "PART_Popup 契约" }, SelectedIndex = 0, Height = 40 }; break;
                    case "ListBox": current = new ListBox { ItemsSource = new[] { "ListBoxItem 选择状态", "ItemTemplate 数据模板", "ItemsPanel 布局模板" }, Height = 125 }; break;
                    case "Slider": current = new Slider { Minimum = 0, Maximum = 100, Value = value.Value }; break;
                    default: current = new ProgressBar { Minimum = 0, Maximum = 100, Value = value.Value, Height = 24 }; break;
                }
                Identify(current, "CurrentControl"); current.IsEnabled = enabled.IsChecked == true; current.VerticalAlignment = VerticalAlignment.Center; host.Content = current;
                string property = name == "TextBox" ? "Text" : name == "Slider" || name == "ProgressBar" ? "Value" : name == "CheckBox" || name == "RadioButton" || name == "ToggleButton" ? "IsChecked" : name == "ComboBox" || name == "ListBox" ? "SelectedItem" : "Command";
                Code("<" + name + " " + property + "=\"{Binding ...}\"/>\n\n// 模板关注点\n" + (name == "TextBox" ? "// PART_ContentHost：承载文本编辑内容" : name == "Slider" ? "// PART_Track：包含 Thumb 与增减按钮" : name == "ComboBox" ? "// PART_Popup：下拉弹出层\n// PART_EditableTextBox：可编辑模式" : name == "ProgressBar" ? "// PART_Track / PART_Indicator：轨道与进度" : "// 保留内容、焦点、禁用和选择反馈"), "当前控件：" + name + "；关键属性：" + property + "。此处使用实际 WPF 控件，可以直接操作。");
            };
            enabled.Checked += (s, e) => { if (current != null) current.IsEnabled = true; }; enabled.Unchecked += (s, e) => { if (current != null) current.IsEnabled = false; };
            value.ValueChanged += (s, e) => { var range = current as RangeBase; if (range != null) range.Value = value.Value; };
            choice.SelectionChanged += (s, e) => update(); update();
        }

        private void Counter()
        {
            var model = new LessonCounterViewModel();
            var counter = Identify(new LessonCounter { DataContext = model, Template = CreateTemplate("<Border Background='#F0ECFC' Padding='20' CornerRadius='12'><StackPanel><TextBlock Text='{Binding Value, RelativeSource={RelativeSource TemplatedParent}}' FontSize='36' Foreground='#7055D5' HorizontalAlignment='Center'/><Button Content='通过 ICommand +1' Command='{Binding IncrementCommand, RelativeSource={RelativeSource TemplatedParent}}' Padding='16,8' Margin='0,16,0,0'/></StackPanel></Border>", "Control") }, "Counter");
            counter.SetBinding(LessonCounter.ValueProperty, new Binding("Value") { Mode = BindingMode.TwoWay });
            counter.SetBinding(LessonCounter.IncrementCommandProperty, new Binding("IncrementCommand"));
            int events = 0; counter.ValueChanged += (s, e) => result.Text = "ViewModel.Value=" + model.Value + "；控件 ValueChanged 路由事件已触发 " + (++events) + " 次。"; Stage(counter, 210);
            var external = Range("外部 Slider → 同一份 ViewModel.Value", "ExternalValue", 0, 20, 0); external.DataContext = model;
            external.SetBinding(RangeBase.ValueProperty, new Binding("Value") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            Actions(Action("重置 ViewModel", () => model.ResetCommand.Execute(null), "Reset"));
            Code("// 自定义控件暴露依赖属性与命令：\npublic static readonly DependencyProperty ValueProperty =\n  DependencyProperty.Register(\"Value\", typeof(int), ...);\n\n<local:LessonCounter Value=\"{Binding Value, Mode=TwoWay}\"\n  IncrementCommand=\"{Binding IncrementCommand}\"/>\n\n// 模板绑定 TemplatedParent.Value / IncrementCommand。\n// ViewModel 只修改数据，不访问视觉元素。", "操作计数器或外部滑块，观察依赖属性、INotifyPropertyChanged、ICommand 与路由事件协作。");
        }

        private void Diagnostics()
        {
            var button = Identify(new Button { Content = "检查我的 Background", Template = CreateTemplate(BasicBody), Foreground = Brushes.White, Height = 58, Padding = new Thickness(16) }, "Preview");
            var style = new Style(typeof(Button)); style.Setters.Add(new Setter(Control.BackgroundProperty, Brush("#7055D5"))); button.Style = style; Stage(button, 120);
            Action inspect = () => { var origin = DependencyPropertyHelper.GetValueSource(button, Control.BackgroundProperty); result.Text = "Background=" + button.Background + "；BaseValueSource=" + origin.BaseValueSource + "；IsExpression=" + origin.IsExpression + "；IsKeyboardFocused=" + button.IsKeyboardFocused; };
            Actions(Action("设置局部值（橙色）", () => { button.Background = Brush("#AB6C26"); inspect(); }, "SetLocal"), Action("ClearValue 恢复样式", () => { button.ClearValue(Control.BackgroundProperty); inspect(); }, "ClearLocal"));
            var virtualize = Check("启用列表虚拟化（500 项）", "Virtualize");
            var list = Identify(new ListBox { Height = 175, ItemsSource = Enumerable.Range(1, 500).Select(i => "诊断条目 " + i).ToArray() }, "VirtualList"); experiment.Children.Add(list);
            Action layout = () =>
            {
                bool on = virtualize.IsChecked == true; VirtualizingPanel.SetIsVirtualizing(list, on); VirtualizingPanel.SetVirtualizationMode(list, VirtualizationMode.Recycling);
                list.ItemsPanel = Parse<ItemsPanelTemplate>("<ItemsPanelTemplate" + Xmlns + "><" + (on ? "VirtualizingStackPanel" : "StackPanel") + "/></ItemsPanelTemplate>");
            };
            Actions(Action("统计已生成的容器", () => { list.UpdateLayout(); int count = Enumerable.Range(0, list.Items.Count).Count(i => list.ItemContainerGenerator.ContainerFromIndex(i) != null); result.Text = "总数据 " + list.Items.Count + " 项；当前已生成 " + count + " 个 ListBoxItem。用 Tab 键同时检查键盘焦点。"; }, "InspectContainers"), Action("检查属性来源", inspect, "InspectValue"));
            virtualize.Checked += (s, e) => layout(); virtualize.Unchecked += (s, e) => layout(); layout();
            Code("var origin = DependencyPropertyHelper.GetValueSource(\n    button, Control.BackgroundProperty);\n// 局部值的优先级高于 Style Setter。\nbutton.ClearValue(Control.BackgroundProperty);\n\n<ListBox VirtualizingPanel.IsVirtualizing=\"True\"\n  VirtualizingPanel.VirtualizationMode=\"Recycling\"/>\n\n// 在 VS 的 Live Visual Tree 检查元素，\n// 在输出窗口检查 Binding 路径与 DataContext。", "本课可直接检查依赖属性值来源、虚拟化容器数量与键盘焦点，而非仅展示排错说明。");
        }

        private void AddKnowledgeCheck(StackPanel root)
        {
            string[][] checks = {
                new[]{"想替换 Button 的视觉结构，应使用什么？", "ControlTemplate", "只设置 FontSize"},
                new[]{"模板中的内容由谁承载？", "ContentPresenter", "只写一个空 Border"},
                new[]{"需要把模板内的编辑写回控件，应选什么？", "Binding + TemplatedParent + TwoWay", "TemplateBinding 本身是双向"},
                new[]{"需要同时满足选中与悬停两个条件，使用什么？", "MultiTrigger", "一个没有条件的 Setter"},
                new[]{"状态间过渡由谁描述？", "VisualTransition", "ItemsPanelTemplate"},
                new[]{"两个模板实例里的同名 Chrome 是同一个对象吗？", "不是，各自有独立名称作用域", "是，同名就共享视觉元素"},
                new[]{"TextBox 文本宿主的约定名称是什么？", "PART_ContentHost", "任意名字都可以"},
                new[]{"要修改列表排列方式，应调整什么？", "ItemsPanel", "只修改 ItemTemplate 的文字"},
                new[]{"替换资源对象后，哪种引用会重新求值？", "DynamicResource", "StaticResource 自动重新查找"},
                new[]{"Slider 的主要数值属性是什么？", "Value", "ContentTemplate"},
                new[]{"ViewModel 的命令应做什么？", "更新数据，让绑定刷新界面", "通过 FindName 直接修改控件内部"},
                new[]{"清除本地 Background 后会怎样？", "重新按优先级取得样式等来源的值", "永远无法恢复 Style Setter"}
            };
            int index = int.Parse(Lesson.Number, CultureInfo.InvariantCulture) - 1;
            var panel = new StackPanel(); panel.Children.Add(Text("本课检查 / " + checks[index][0], 15));
            var feedback = Identify(Text("选择一个答案，检查刚才的实验结论。", 12, "#777486"), "CheckFeedback");
            var answers = new WrapPanel();
            // Alternate positions so the correct option isn't always the first action.
            var right = Action(checks[index][1], () => feedback.Text = "✓ 正确。" + checks[index][1] + "。", "CorrectAnswer");
            var wrong = Action(checks[index][2], () => feedback.Text = "再观察一次实验。正确理解是：" + checks[index][1] + "。", "WrongAnswer");
            answers.Children.Add(index % 2 == 0 ? right : wrong); answers.Children.Add(index % 2 == 0 ? wrong : right);
            panel.Children.Add(answers); panel.Children.Add(feedback); root.Children.Add(Card(panel));
        }
    }

    public sealed class LearningCard
    {
        public LearningCard(string title, string detail) { Title = title; Detail = detail; }
        public string Title { get; }
        public string Detail { get; }
    }

    public sealed class LessonCounter : Control
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(LessonCounter),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((LessonCounter)d).RaiseEvent(new RoutedEventArgs(ValueChangedEvent))));
        public static readonly DependencyProperty IncrementCommandProperty = DependencyProperty.Register("IncrementCommand", typeof(ICommand), typeof(LessonCounter));
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LessonCounter));
        public int Value { get => (int)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public ICommand IncrementCommand { get => (ICommand)GetValue(IncrementCommandProperty); set => SetValue(IncrementCommandProperty, value); }
        public event RoutedEventHandler ValueChanged { add => AddHandler(ValueChangedEvent, value); remove => RemoveHandler(ValueChangedEvent, value); }
    }
}
