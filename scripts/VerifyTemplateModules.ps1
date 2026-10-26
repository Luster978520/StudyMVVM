param(
    [string]$AssemblyPath = (Join-Path $PSScriptRoot '..\StudyMVVM\bin\Debug\StudyMVVM.exe'),
    [string]$OutputDirectory = ''
)
# Run with Windows PowerShell -STA after building the .NET Framework project.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName PresentationFramework, PresentationCore, WindowsBase, System.Xaml
$testAssembly = Join-Path $env:TEMP ('TemplateModules-' + [guid]::NewGuid().ToString('N') + '.dll')
Copy-Item -LiteralPath (Resolve-Path $AssemblyPath).Path -Destination $testAssembly
$assembly = [Reflection.Assembly]::LoadFrom($testAssembly)
[System.Windows.Application]::ResourceAssembly = $assembly
$references = @($testAssembly, [System.Windows.Window].Assembly.Location, [System.Windows.Media.Visual].Assembly.Location, [System.Windows.Threading.Dispatcher].Assembly.Location, [System.Xaml.XamlReader].Assembly.Location)
Add-Type -ReferencedAssemblies $references -TypeDefinition @'
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using StudyMVVM.Views;
using StudyMVVM.ViewModel;

public static class ModuleVerification
{
    static void Assert(bool ok, string message) { if (!ok) throw new Exception(message); }
    static IEnumerable<DependencyObject> All(DependencyObject root)
    {
        for (int i=0; i<VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child=VisualTreeHelper.GetChild(root,i); yield return child;
            foreach(var n in All(child)) yield return n;
        }
    }
    static T Find<T>(DependencyObject root, string id) where T: DependencyObject
    { return All(root).OfType<T>().Single(n => AutomationProperties.GetAutomationId(n)==id); }
    static void Layout(FrameworkElement root)
    {
        root.Measure(new Size(1440,1300)); root.Arrange(new Rect(0,0,1440,1300)); root.UpdateLayout();
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle,new Action(()=>{})); root.UpdateLayout();
    }
    static void Click(FrameworkElement root, string id)
    { Find<Button>(root,id).RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Layout(root); }
    static void Choose(FrameworkElement root,string id,int index)
    { Find<ComboBox>(root,id).SelectedIndex=index; Layout(root); }
    static Border Chrome(Button b) { b.ApplyTemplate(); return (Border)b.Template.FindName("Chrome",b); }
    public static string Run(string output)
    {
        var app=new Application(); var log=new StringWriter(); var listener=new TextWriterTraceListener(log);
        PresentationTraceSources.DataBindingSource.Listeners.Add(listener);
        PresentationTraceSources.DataBindingSource.Switch.Level=SourceLevels.Error;
        var window=new TemplateLabWindow(); var vm=(TemplateLabViewModel)window.DataContext;
        var root=(FrameworkElement)window.Content; Layout(root);
        var codes=new HashSet<string>();
        foreach(var lesson in vm.Lessons)
        {
            vm.OpenLessonCommand.Execute(lesson); Layout(root);
            var workspace=All(root).OfType<TemplateLessonWorkspace>().Single();
            Assert(vm.IsLesson && workspace.ModuleNumber==lesson.Number,"Wrong route for module "+lesson.Number);
            string p="Module"+lesson.Number+"/"; codes.Add(workspace.CurrentCode);
            switch(lesson.Number)
            {
                case "01":
                    Choose(root,p+"Mode",2);
                    Assert(All(Find<Button>(root,p+"Preview")).OfType<System.Windows.Shapes.Ellipse>().Any(),"01: template structure did not change");
                    Click(root,p+"Preview"); Assert(workspace.LastResult.Contains("Click"),"01: Button behavior missing"); break;
                case "02":
                    Find<Slider>(root,p+"Padding").Value=28; Layout(root);
                    var two=Find<Button>(root,p+"Preview"); Assert(two.Padding.Left==28,"02: Padding not forwarded");
                    Find<CheckBox>(root,p+"ContentHost").IsChecked=false; Layout(root);
                    Assert(((Border)VisualTreeHelper.GetChild(two,0)).Child==null,"02: ContentPresenter was not removed"); break;
                case "03":
                    Choose(root,p+"BindingMode",2);
                    var parent=Find<Button>(root,p+"Preview"); parent.ApplyTemplate();
                    var inner=(TextBox)parent.Template.FindName("InnerEditor",parent);
                    inner.SetCurrentValue(TextBox.TextProperty,"two-way-test"); inner.GetBindingExpression(TextBox.TextProperty).UpdateSource(); Layout(root);
                    Assert(Find<TextBox>(root,p+"Source").Text=="two-way-test","03: two-way edit did not reach external source");
                    Choose(root,p+"BindingMode",0); parent.ApplyTemplate();
                    inner=(TextBox)parent.Template.FindName("InnerEditor",parent); inner.SetCurrentValue(TextBox.TextProperty,"one-way-only"); Layout(root);
                    Assert(Find<TextBox>(root,p+"Source").Text=="two-way-test","03: TemplateBinding wrote backwards"); break;
                case "04":
                    var toggle=Find<ToggleButton>(root,p+"Preview"); toggle.IsChecked=true; Layout(root);
                    var tborder=(Border)toggle.Template.FindName("Chrome",toggle);
                    Assert(tborder.Background.ToString()=="#FF208261","04: checked trigger missing");
                    Find<CheckBox>(root,p+"Enabled").IsChecked=false; Layout(root);
                    Assert(!toggle.IsEnabled && tborder.Background.ToString()=="#FFCDC8DF","04: disabled trigger missing"); break;
                case "05":
                    Click(root,p+"Expanded");
                    var motion=All(workspace).OfType<Border>().Single(b=>b.Name=="MotionRoot");
                    var group=(VisualStateGroup)VisualStateManager.GetVisualStateGroups(motion)[0];
                    Assert(group.CurrentState.Name=="Expanded","05: real VSM state did not change");
                    Find<Slider>(root,p+"Duration").Value=0; Layout(root);
                    Click(root,p+"Done"); Assert(group.CurrentState.Name=="Done","05: VSM completion failed"); break;
                case "06":
                    Click(root,p+"FindA"); var firstBorder=Chrome(Find<Button>(root,p+"InstanceA")); var secondBorder=Chrome(Find<Button>(root,p+"InstanceB"));
                    Assert(!ReferenceEquals(firstBorder,secondBorder) && firstBorder.BorderThickness.Left==4 && secondBorder.BorderThickness.Left==0,"06: namescope isolation failed");
                    Click(root,p+"FindB"); Assert(secondBorder.BorderThickness.Left==4 && firstBorder.BorderThickness.Left==0,"06: wrong instance highlighted"); break;
                case "07":
                    var input=Find<TextBox>(root,p+"TextInput");
                    Assert(input.Template.FindName("PART_ContentHost",input)!=null,"07: valid content host absent");
                    Find<CheckBox>(root,p+"KeepPart").IsChecked=false; Layout(root);
                    Assert(input.Template.FindName("PART_ContentHost",input)==null,"07: missing host experiment failed");
                    Find<CheckBox>(root,p+"KeepPart").IsChecked=true; Layout(root);
                    Assert(input.Template.FindName("PART_ContentHost",input)!=null,"07: host restoration failed"); break;
                case "08":
                    Choose(root,p+"ItemTemplate",1); Choose(root,p+"ItemsPanel",1);
                    var list=Find<ListBox>(root,p+"List");
                    Assert(list.Items.Count==3 && All(list).OfType<WrapPanel>().Any(),"08: item layout did not switch");
                    var wrap=All(list).OfType<WrapPanel>().Single();
                    Assert(wrap.ActualWidth<=list.ActualWidth,"08: WrapPanel was measured with infinite horizontal space"); break;
                case "09":
                    Click(root,p+"Green"); var dyn=Find<Button>(root,p+"Dynamic"); var stat=Find<Button>(root,p+"Static");
                    Assert(dyn.Background.ToString()=="#FF208261" && stat.Background.ToString()=="#FF7055D5","09: resource replacement semantics incorrect");
                    Click(root,p+"ResolveStatic"); Assert(stat.Background.ToString()==dyn.Background.ToString(),"09: static resource relookup failed"); break;
                case "10":
                    for(int i=0;i<9;i++) Choose(root,p+"ControlType",i);
                    Find<Slider>(root,p+"Value").Value=72; Layout(root);
                    Assert(Find<ProgressBar>(root,p+"CurrentControl").Value==72,"10: progress value failed");
                    Find<CheckBox>(root,p+"Enabled").IsChecked=false; Layout(root);
                    Assert(!Find<Control>(root,p+"CurrentControl").IsEnabled,"10: disable failed"); break;
                case "11":
                    var counter=Find<LessonCounter>(root,p+"Counter"); counter.IncrementCommand.Execute(null); Layout(root);
                    Assert(counter.Value==1,"11: MVVM command did not update dependency property");
                    Find<Slider>(root,p+"ExternalValue").Value=7; Layout(root);
                    Assert(counter.Value==7 && workspace.LastResult.Contains("ValueChanged"),"11: external binding or routed event failed");
                    Click(root,p+"Reset"); Assert(counter.Value==0,"11: reset failed"); break;
                case "12":
                    Click(root,p+"SetLocal"); var inspect=Find<Button>(root,p+"Preview");
                    Assert(DependencyPropertyHelper.GetValueSource(inspect,Control.BackgroundProperty).BaseValueSource==BaseValueSource.Local,"12: local precedence missing");
                    Click(root,p+"ClearLocal"); Assert(DependencyPropertyHelper.GetValueSource(inspect,Control.BackgroundProperty).BaseValueSource==BaseValueSource.Style,"12: style fallback failed");
                    var virtualList=Find<ListBox>(root,p+"VirtualList");
                    int before=Enumerable.Range(0,500).Count(i=>virtualList.ItemContainerGenerator.ContainerFromIndex(i)!=null);
                    Find<CheckBox>(root,p+"Virtualize").IsChecked=false; Layout(root);
                    int after=Enumerable.Range(0,500).Count(i=>virtualList.ItemContainerGenerator.ContainerFromIndex(i)!=null);
                    Assert(before<500 && after==500,"12: virtualization comparison did not affect container generation"); break;
            }
            Click(root,p+"WrongAnswer"); Click(root,p+"CorrectAnswer");
            Assert(Find<TextBlock>(root,p+"CheckFeedback").Text.StartsWith("\u2713"),"Knowledge check failed on "+lesson.Number);
            vm.CompleteLessonCommand.Execute(null); Layout(root);
            Assert(vm.IsLesson && workspace.ModuleNumber==lesson.Number,"Completing a module redirected to a shared page");
            if(!string.IsNullOrWhiteSpace(output))
            {
                Directory.CreateDirectory(output); var image=new RenderTargetBitmap(1440,1300,96,96,PixelFormats.Pbgra32); image.Render(root);
                var png=new PngBitmapEncoder(); png.Frames.Add(BitmapFrame.Create(image));
                using(var stream=File.Create(Path.Combine(output,"module-"+lesson.Number+".png"))) png.Save(stream);
            }
        }
        Assert(codes.Count==12,"Some modules share the same example instead of a dedicated workspace");
        Assert(vm.Progress==100,"Progress failed"); vm.CompleteLessonCommand.Execute(null); Assert(vm.Progress==100,"Duplicate completion increased progress");
        vm.ChangeLessonCommand.Execute(-1); Assert(vm.SelectedLesson.Number=="11","Previous module route failed");
        vm.ChangeLessonCommand.Execute(1); Assert(vm.SelectedLesson.Number=="12" && !vm.HasNextLesson,"Next/end boundary failed");
        listener.Flush(); Assert(string.IsNullOrWhiteSpace(log.ToString()),"Unexpected WPF binding errors: "+log);
        window.Close(); app.Shutdown();
        return "PASS: 12 unique routes, native controls and actions in every module, two-way binding, VSM, isolated namescopes, template parts, item layout, resource replacement, 9 control types, MVVM/routed events, virtualization, 12 quizzes, completion and previous/next navigation; zero binding errors.";
    }
}
'@
[ModuleVerification]::Run($OutputDirectory)
