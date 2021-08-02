using UIKit;

namespace BTProgressHUDDemo
{
    public class ModalView : UIViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;
            Title = "I'm a modal!";

            var button = new UIButton(UIButtonType.RoundedRect)
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            button.SetTitle("Show Hud", UIControlState.Normal);

            Add(button);

            button.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 20).Active = true;
            button.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            button.TouchUpInside += (s, e) =>
            {
                BTProgressHUD.BTProgressHUD.Show("Cancel", () => { }, "Hello from modal", maskType: BTProgressHUD.MaskType.Black);
            };
        }
    }
}
