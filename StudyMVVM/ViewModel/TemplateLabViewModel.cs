using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace StudyMVVM.ViewModel
{
    public sealed class TemplateLesson
    {
        public string Number { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Topics { get; set; }
        public string MoreTopics { get; set; }
        public string Explanation { get; set; }
        public string ExperimentPage { get; set; }
    }

    public sealed class TemplateLabViewModel : INotifyPropertyChanged
    {
        private string page = "lab";
        private double radius = 8;
        private string color = "#7055D5";
        private string selectedState = "Normal";
        private string status = "点击参数，观察模板如何改变控件外观。";
        private bool quizChecked;
        private bool quizCorrect;
        private int previewClicks;
        private readonly HashSet<string> completed = new HashSet<string>();

        public TemplateLabViewModel()
        {
            Lessons = CreateLessons();
            SelectedLesson = Lessons[2];
            NavigateCommand = new LabCommand(p => Navigate(Convert.ToString(p)));
            RadiusCommand = new LabCommand(p => Radius = Convert.ToDouble(p, CultureInfo.InvariantCulture));
            ColorCommand = new LabCommand(p => { color = Convert.ToString(p); RefreshPreview(); });
            StateCommand = new LabCommand(p =>
            {
                selectedState = Convert.ToString(p);
                Changed("SelectedState", "IsNormal", "IsMouseOverState", "IsPressedState", "IsDisabledState",
                    "StateDescription", "StateProperty", "StateCode", "StateBackground", "StateEnabled");
                Status = "当前示例状态：" + selectedState;
            });
            PreviewCommand = new LabCommand(p => Status = "Click 事件已触发 " + (++previewClicks) + " 次；替换模板后，Button 的点击行为依然保留。");
            OpenLessonCommand = new LabCommand(p =>
            {
                var lesson = p as TemplateLesson;
                if (lesson == null) return;
                SelectedLesson = lesson;
                Changed("SelectedLesson");
                Navigate("lesson");
            });
            CompleteLessonCommand = new LabCommand(p =>
            {
                completed.Add(SelectedLesson.Number);
                Changed("Progress", "ProgressText");
                Navigate(SelectedLesson.ExperimentPage);
            });
            AnswerCommand = new LabCommand(p =>
            {
                quizCorrect = Convert.ToString(p) == "B";
                quizChecked = true;
                if (quizCorrect) completed.Add("03");
                Changed("QuizChecked", "QuizPending", "QuizTitle", "QuizExplanation", "QuizResultBackground", "Progress", "ProgressText");
                Status = quizCorrect ? "回答正确：模板属性连接成功。" : "试着从模板所属的控件寻找 Background。";
            });
            RetryCommand = new LabCommand(p => { quizChecked = false; Changed("QuizChecked", "QuizPending"); });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Changed(params string[] names)
        {
            foreach (var name in names) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public IList<TemplateLesson> Lessons { get; }
        public TemplateLesson SelectedLesson { get; private set; }
        public ICommand NavigateCommand { get; }
        public ICommand RadiusCommand { get; }
        public ICommand ColorCommand { get; }
        public ICommand StateCommand { get; }
        public ICommand PreviewCommand { get; }
        public ICommand OpenLessonCommand { get; }
        public ICommand CompleteLessonCommand { get; }
        public ICommand AnswerCommand { get; }
        public ICommand RetryCommand { get; }

        public bool IsRoadmap => page == "roadmap";
        public bool IsLab => page == "lab";
        public bool IsTree => page == "tree";
        public bool IsStates => page == "states";
        public bool IsChallenge => page == "challenge";
        public bool IsLesson => page == "lesson";
        public bool LabNavSelected => IsLab || IsTree;
        public bool RoadmapNavSelected => IsRoadmap || IsLesson;
        public string Status { get => status; set { status = value; Changed("Status"); } }
        public double Progress => completed.Count * 100.0 / Lessons.Count;
        public string ProgressText => "本次已完成  " + completed.Count + " / 12 模块";

        public string SectionName
        {
            get
            {
                if (IsRoadmap || IsLesson) return "Learning path / 学习路线";
                if (IsTree) return "Visual anatomy / 模板拆解";
                if (IsStates) return "States & motion / 状态与动画";
                if (IsChallenge) return "Practice / 实战挑战";
                return "Template playground / 模板实验室";
            }
        }
        public string Eyebrow => SectionName.ToUpperInvariant();
        public string PageTitle
        {
            get
            {
                if (IsRoadmap) return "把控件模板，真正学明白。";
                if (IsTree) return "沿着视觉树，找到每一层的职责。";
                if (IsStates) return "让每一次操作，都有清晰的回应。";
                if (IsChallenge) return "把知识，变成你自己的模板。";
                if (IsLesson) return SelectedLesson.Number + "  " + SelectedLesson.Title;
                return "外观可以重写，行为依然保留。";
            }
        }
        public string PageSubtitle
        {
            get
            {
                if (IsRoadmap) return "从看懂一棵视觉树，到独立写出有状态、有主题的控件。每一课，都可以动手试一试。";
                if (IsTree) return "MODULE 06 / 对照 Border、ContentPresenter 与模板中的属性连接。";
                if (IsStates) return "MODULE 04–05 / 比较 Trigger 与 VisualStateManager，理解状态、属性和动画之间的关系。";
                if (IsChallenge) return "CHALLENGE 01 / 完成一个可复用的 Button 模板，让外部 Background 属性真正生效。";
                if (IsLesson) return SelectedLesson.Summary;
                return "MODULE 03 / 通过 TemplateBinding，把 Button 的 Background 传给模板中的 Border。";
            }
        }

        private void Navigate(string destination)
        {
            if (!new[] { "roadmap", "lab", "tree", "states", "challenge", "lesson" }.Contains(destination)) return;
            page = destination;
            Changed("IsRoadmap", "IsLab", "IsTree", "IsStates", "IsChallenge", "IsLesson", "LabNavSelected", "RoadmapNavSelected",
                "SectionName", "Eyebrow", "PageTitle", "PageSubtitle");
        }

        public double Radius
        {
            get => radius;
            set { radius = Math.Round(Math.Max(0, Math.Min(32, value))); RefreshPreview(); }
        }
        public CornerRadius PreviewRadius => new CornerRadius(radius);
        public string BackgroundHex => color;
        public Brush PreviewBackground => BrushFrom(color);
        private void RefreshPreview()
        {
            Changed("Radius", "PreviewRadius", "BackgroundHex", "PreviewBackground", "TemplateCode", "CodeLines");
        }

        public string TemplateCode => @"<ControlTemplate TargetType=""{x:Type Button}"">
  <Border x:Name=""Chrome""
      Background=""{TemplateBinding Background}""
      BorderBrush=""{TemplateBinding BorderBrush}""
      BorderThickness=""{TemplateBinding BorderThickness}""
      CornerRadius=""__RADIUS__"">
    <ContentPresenter
        Margin=""{TemplateBinding Padding}""
        HorizontalAlignment=
          ""{TemplateBinding HorizontalContentAlignment}""
        VerticalAlignment=
          ""{TemplateBinding VerticalContentAlignment}""
        Content=""{TemplateBinding Content}""
        ContentTemplate=""{TemplateBinding ContentTemplate}"" />
  </Border>
</ControlTemplate>".Replace("__RADIUS__", radius.ToString(CultureInfo.InvariantCulture));
        public IEnumerable<LabCodeLine> CodeLines => TemplateCode.Split('\n').Select((line, i) => new LabCodeLine
        {
            Number = (i + 1).ToString().PadLeft(2), Text = line.TrimEnd('\r'),
            Color = BrushFrom(line.Contains("TemplateBinding") ? "#B49CF5" : "#DCD9F0")
        });

        public string SelectedState => selectedState;
        public bool IsNormal => selectedState == "Normal";
        public bool IsMouseOverState => selectedState == "MouseOver";
        public bool IsPressedState => selectedState == "Pressed";
        public bool IsDisabledState => selectedState == "Disabled";
        public bool StateEnabled => !IsDisabledState;
        public Brush StateBackground => BrushFrom(IsDisabledState ? "#CDC8DF" : IsPressedState ? "#422D96" : IsMouseOverState ? "#5940B8" : "#7055D5");
        public string StateProperty => IsDisabledState ? "IsEnabled = False" : IsPressedState ? "IsPressed = True" : IsMouseOverState ? "IsMouseOver = True" : "IsEnabled = True";
        public string StateDescription => IsDisabledState ? "禁用时提供弱化外观，并停止接收正常的按钮操作。" : IsPressedState ? "鼠标按下时，按下状态覆盖悬停外观；释放后恢复。" : IsMouseOverState ? "指针进入控件后，触发悬停反馈，提示它可以被操作。" : "默认状态。没有悬停或按下时，显示基础外观。";
        public string StateCode => IsNormal ? @"<!-- 默认外观由模板根元素提供 -->
<Border x:Name=""Chrome""
    Background=""{TemplateBinding Background}"">
  <ContentPresenter />
</Border>

<!-- 切换上方标签，查看状态 Trigger。 -->" : @"<ControlTemplate.Triggers>
  <Trigger Property=""__PROPERTY__""
           Value=""__VALUE__"">
    <Setter TargetName=""Chrome""
            Property=""Background""
            Value=""__COLOR__"" />
  </Trigger>
</ControlTemplate.Triggers>".Replace("__PROPERTY__", IsDisabledState ? "IsEnabled" : IsPressedState ? "IsPressed" : "IsMouseOver")
            .Replace("__VALUE__", IsDisabledState ? "False" : "True")
            .Replace("__COLOR__", IsDisabledState ? "#CDC8DF" : IsPressedState ? "#422D96" : "#5940B8");

        public string ChallengeCode => @"<ControlTemplate TargetType=""{x:Type Button}"">
  <Border CornerRadius=""8""
          Background=""  ?  "">
    <ContentPresenter />
  </Border>
</ControlTemplate>";
        public bool QuizChecked => quizChecked;
        public bool QuizPending => !quizChecked;
        public string QuizTitle => quizCorrect ? "✓  选对了，模板与控件连接成功。" : "↻  再想一步：这个属性应该从哪里来？";
        public string QuizExplanation => quizCorrect
            ? "TemplateBinding 将 Border.Background 连接到模板所属的 Button.Background。外部修改控件背景时，模板内部同步变化。它是单向绑定；需要双向更新或转换器时，请使用 Binding + RelativeSource TemplatedParent。"
            : "普通 Binding 默认从 DataContext 查找 Background；写死 Purple 只能得到固定颜色。这里需要从模板所属的 Button 读取属性，请选择 {TemplateBinding Background}。";
        public Brush QuizResultBackground => BrushFrom(quizCorrect ? "#E9F6EF" : "#FFF4E4");
        private static Brush BrushFrom(string hex) => (Brush)new BrushConverter().ConvertFromInvariantString(hex);

        private static IList<TemplateLesson> CreateLessons()
        {
            var rows = new[]
            {
                new[] { "认识控件模板", "Style 与 Template 的职责", "ControlTemplate · TargetType · 视觉树", "Style / ControlTemplate / DataTemplate", "Style 通过 Setter 配置属性，ControlTemplate 定义控件的视觉结构。替换 Template 会替换整棵模板视觉树；Button 的 Click、Command 等行为仍由控件类提供。DataTemplate 则描述数据如何显示，不负责替换控件外壳。", "lab" },
                new[] { "搭建第一份模板", "用 Border 与内容宿主重塑 Button", "ContentPresenter · Padding · 对齐", "Content · ContentTemplate · 继承属性", "先指定 TargetType，再用 Border 构建外壳、ContentPresenter 承载内容。通过 TemplateBinding 传递 Background、Padding 与内容对齐。不要把控件文字写死在模板中；保留 Content 和 ContentTemplate，才能复用文字、图标或复杂内容。", "lab" },
                new[] { "模板属性传递", "从控件属性连接到内部元素", "TemplateBinding · TemplatedParent", "依赖属性 · OneWay / TwoWay · 优先级", "TemplateBinding 是针对模板的轻量单向绑定，连接模板内部属性与模板所属控件的依赖属性。完整 Binding 可用 RelativeSource TemplatedParent 显式指定控件作为来源，并支持转换器及 TwoWay。外部属性能否生效，还取决于依赖属性优先级。", "lab" },
                new[] { "触发器与交互", "为鼠标、焦点与选择提供反馈", "Trigger · MultiTrigger · DataTrigger", "MultiDataTrigger · EventTrigger · 优先级", "Trigger 检查依赖属性，DataTrigger 检查绑定值；多条件版本要求所有条件满足。EventTrigger 通过事件启动 Storyboard。Button 的 IsMouseOver 与 IsPressed 可能同时为真，需明确 Setter 的覆盖顺序，并为 IsEnabled=False 与键盘焦点保留反馈。", "states" },
                new[] { "视觉状态与动画", "把每次状态变化变成清晰反馈", "VisualStateManager · VisualStateGroup", "VisualTransition · Storyboard · 缓动", "在模板根元素声明 VisualStateGroups。CommonStates 包含 Normal、MouseOver、Pressed、Disabled；同组一次只激活一个状态，FocusStates 则独立工作。VisualTransition 描述过渡，Storyboard 描述属性动画。标准控件按约定驱动状态，自定义控件可调用 GoToState。", "states" },
                new[] { "模板树与名称作用域", "看懂模板生成的每一个元素", "逻辑树 / 视觉树 · x:Name · NameScope", "FindName · GetTemplateChild · ApplyTemplate", "逻辑树描述内容关系，视觉树还包含模板生成的元素。每个模板实例都有独立名称作用域。模板外部可先 ApplyTemplate，再调用 control.Template.FindName(name, control)；控件内部通常在 OnApplyTemplate 中调用 GetTemplateChild。查找结果必须判空。", "tree" },
                new[] { "模板部件与控件契约", "换外观时保留必要的功能", "TemplatePart · PART_ContentHost", "PART_Track · OnApplyTemplate · 空值处理", "控件可能依赖特定命名部件：TextBox 的 PART_ContentHost 承载文本，Slider 的 PART_Track 负责轨道。按对应控件文档保留部件名称与类型。OnApplyTemplate 可能多次执行，应解绑旧部件事件、重新查找并绑定新部件，同时处理部件缺失。", "tree" },
                new[] { "内容与列表模板", "分清控件、数据和布局的模板", "DataTemplate · ItemTemplate · ItemsPanel", "ItemsPresenter · ItemContainerStyle · Selector", "ControlTemplate 定义控件外壳；DataTemplate 定义一项数据的呈现；ItemsPanelTemplate 定义多项的布局。ItemsPresenter 在模板内承载集合项区域。ItemContainerStyle 配置 ListBoxItem 等容器，TemplateSelector 按数据选择模板。修改列表模板时还要检查滚动与虚拟化。", "tree" },
                new[] { "资源、样式与主题", "让模板在整个应用中复用", "ResourceDictionary · BasedOn · 隐式样式", "StaticResource / DynamicResource · Generic.xaml", "将共享模板与画刷放进 ResourceDictionary，并通过合并字典组织。BasedOn 复用样式；无 x:Key 的目标类型样式可隐式应用。StaticResource 在查找时取得资源，DynamicResource 支持后续资源变化。自定义控件默认样式通常位于 Themes/Generic.xaml，并配合 DefaultStyleKey。", "lab" },
                new[] { "常用控件实战", "从 Button 扩展到输入与选择", "Button · ToggleButton · CheckBox · RadioButton", "TextBox · ComboBox · ListBox · Slider · ProgressBar", "先从 Button 的内容、悬停、按下和禁用开始。再处理 ToggleButton 的 IsChecked、TextBox 的内容宿主、ComboBox 的弹出层、Slider 的轨道和 ProgressBar 的进度显示。每类控件都有自己的状态与模板部件契约，不能直接用同一份 Border 模板替换所有控件。", "challenge" },
                new[] { "自定义控件与 MVVM", "连接外观、命令和控件逻辑", "CustomControl / UserControl · DefaultStyleKey", "DependencyProperty · ICommand · RoutedEvent", "UserControl 适合组合现有控件；需要可替换外观时通常派生自 Control。用依赖属性暴露可绑定参数、ICommand 承载操作、路由事件通知交互。模板负责视觉呈现，ViewModel 负责页面状态和命令；避免在模板里绑定到固定窗口或写死业务数据。", "challenge" },
                new[] { "调试与质量检查", "交付稳定、可访问的控件模板", "Live Visual Tree · 属性优先级 · 绑定诊断", "键盘焦点 · 高对比度 · 虚拟化 · 动画性能", "在 Live Visual Tree 中检查实际模板元素，并查看依赖属性的有效值来源。绑定失败时检查输出窗口、DataContext 与名称作用域。通过 Tab、空格和回车验证键盘操作，检查高对比度、禁用状态与焦点提示。大列表保留虚拟化，避免过度复杂的视觉树和无休止动画。", "challenge" }
            };
            return rows.Select((r, i) => new TemplateLesson { Number = (i + 1).ToString("00"), Title = r[0], Summary = r[1], Topics = r[2], MoreTopics = r[3], Explanation = r[4], ExperimentPage = r[5] }).ToList();
        }
    }

    public sealed class LabCodeLine
    {
        public string Number { get; set; }
        public string Text { get; set; }
        public Brush Color { get; set; }
    }

    // A horizontal ScrollViewer measures with infinite width. Constrain the workspace
    // to its viewport so star columns and wrapped lesson cards keep their proportions.
    public sealed class LabContentWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Max(880, value is double ? (double)value - 64 : 880);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class LabCommand : ICommand
    {
        private readonly Action<object> execute;
        public LabCommand(Action<object> execute) { this.execute = execute; }
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => execute(parameter);
        public event EventHandler CanExecuteChanged { add { } remove { } }
    }
}
