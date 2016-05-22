using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ViewLibrary
{
    public class CustomGridView : ListView
    {
        static CustomGridView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomGridView), new FrameworkPropertyMetadata(typeof(CustomGridView)));
        }

        public CustomGridView()
        {
            this.SizeChanged += CustomGridView_SizeChanged;
        }

        // To bind grid item simply specify Width="{Binding ElementName=[GridView Name], Path = ItemWidth}" in ItemTemplate
        // Item can also be wrapped in ViewBox to improve scale performance

        private async void CustomGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newVal = e.NewSize.Width - ItemsPanelPadding.Left - ItemsPanelPadding.Right;
            if (newVal >= 0)
            {
                ProcessGridViewSizeChange(newVal);
            }
        }

        void ProcessGridViewSizeChange(double updatedWidth)
        {
            int count = Math.Min(MaxCountHorizontal, Math.Max(MinCountHorizontal, (int)(updatedWidth / IntendedWidth)));
            if (count > 0)
            {
                var newWidth = Math.Max(0, (updatedWidth / count) - 0.3);
                if (count < MaxCountHorizontal)
                {
                    if (newWidth > (IntendedWidth * (1 + MaxWidthIncrementPercent)))
                    {
                        count++;
                    }
                }
                ProcessCount(updatedWidth, count);
            }
        }

        void ProcessCount(double updatedWidth, int count)
        {
            var newWidth = Math.Max(0, (updatedWidth / count) - 0.3);
            if (Math.Abs(ItemWidth - newWidth) > 0.1)
            {
                ItemWidth = newWidth;
                if (!IsHeightFixed)
                    ItemHeight = ((IntendedHeight / IntendedWidth) * ItemWidth);
            }
        }

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(CustomGridView), new PropertyMetadata(0.0));

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(CustomGridView), new PropertyMetadata(0.0));


        public double IntendedHeight
        {
            get { return (double)GetValue(IntendedHeightProperty); }
            set { SetValue(IntendedHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IntendedHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntendedHeightProperty =
            DependencyProperty.Register("IntendedHeight", typeof(double), typeof(CustomGridView), new PropertyMetadata(0.0, IntendedHeightChanged));

        private static void IntendedHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CustomGridView).ItemHeight = (double)e.NewValue;
        }

        public double IntendedWidth
        {
            get { return (double)GetValue(IntendedWidthProperty); }
            set { SetValue(IntendedWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IntendedWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntendedWidthProperty =
            DependencyProperty.Register("IntendedWidth", typeof(double), typeof(CustomGridView), new PropertyMetadata(0.0, IntendedWidthChanged));

        private static void IntendedWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CustomGridView).ItemWidth = (double)e.NewValue;
        }

        public int MaxCountHorizontal
        {
            get { return (int)GetValue(MaxCountHorizontalProperty); }
            set { SetValue(MaxCountHorizontalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxCountHorizontal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxCountHorizontalProperty =
            DependencyProperty.Register("MaxCountHorizontal", typeof(int), typeof(CustomGridView), new PropertyMetadata(int.MaxValue));



        public int MinCountHorizontal
        {
            get { return (int)GetValue(MinCountHorizontalProperty); }
            set { SetValue(MinCountHorizontalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinCountHorizontal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinCountHorizontalProperty =
            DependencyProperty.Register("MinCountHorizontal", typeof(int), typeof(CustomGridView), new PropertyMetadata(1));


        public bool IsHeightFixed
        {
            get { return (bool)GetValue(IsHeightFixedProperty); }
            set { SetValue(IsHeightFixedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHeightFixed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHeightFixedProperty =
            DependencyProperty.Register("IsHeightFixed", typeof(bool), typeof(CustomGridView), new PropertyMetadata(true));

        public Thickness ItemsPanelPadding
        {
            get { return (Thickness)GetValue(ItemsPanelPaddingProperty); }
            set { SetValue(ItemsPanelPaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHeightFixed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsPanelPaddingProperty =
            DependencyProperty.Register("ItemsPanelPadding", typeof(Thickness), typeof(CustomGridView), new PropertyMetadata(new Thickness(0, 0, 0, 0)));



        public double MaxWidthIncrementPercent
        {
            get { return (double)GetValue(MaxWidthIncrementPercentProperty); }
            set { SetValue(MaxWidthIncrementPercentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxWidthIncrementPercent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxWidthIncrementPercentProperty =
            DependencyProperty.Register("MaxWidthIncrementPercent", typeof(double), typeof(CustomGridView), new PropertyMetadata(0.25));


    }
}
