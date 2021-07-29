#pragma once
#include "TreeNode2.h"
#include "ViewModel.h"
#include "TreeView2Item.h"

namespace TreeViewControl {
    public ref class TreeView2ItemClickEventArgs sealed
    {
    public:
        TreeView2ItemClickEventArgs() {}

        property Object^ ClickedItem
        {
            Object^ get() { return clickedItem; };
            void set(Object^ value) { clickedItem = value; };
        }

        property bool IsHandled
        {
            bool get() { return isHandled; };
            void set(bool value) { isHandled = value; };
        }
    private:
        Object^ clickedItem = nullptr;
        bool isHandled = false;
    };

    ref class TreeView2;
    [Windows::Foundation::Metadata::WebHostHidden]
    public delegate void TreeView2ItemClickHandler(TreeView2^ sender, TreeView2ItemClickEventArgs^ args);

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class TreeView2 sealed : Windows::UI::Xaml::Controls::ListView
    {
    public:
        TreeView2();

        //This event is used to expose an alternative to itemclick to developers.
        event TreeView2ItemClickHandler^ TreeView2ItemClick;

        //This RootNode property is used by the TreeView2 to handle additions into the TreeView2 and
        //accurate VectorChange with multiple 'root level nodes'. This node will not be placed
        //in the flatViewModel, but has it's vectorchanged event hooked up to flatViewModel's
        //handler.
        property TreeNode2^ RootNode
        {
            TreeNode2^ get() { return rootNode; };
        }

        void TreeView2_OnItemClick(Platform::Object^ sender, Windows::UI::Xaml::Controls::ItemClickEventArgs^ args);

        void TreeView2_DragItemsStarting(Platform::Object^ sender, Windows::UI::Xaml::Controls::DragItemsStartingEventArgs^ e);

        void TreeView2_DragItemsCompleted(Windows::UI::Xaml::Controls::ListViewBase^ sender, Windows::UI::Xaml::Controls::DragItemsCompletedEventArgs^ args);

        void ExpandNode(TreeNode2^ targetNode);

        void CollapseNode(TreeNode2^ targetNode);

    protected:
        void PrepareContainerForItemOverride(DependencyObject^ element, Object^ item) override;
        Windows::UI::Xaml::DependencyObject^ GetContainerForItemOverride() override;

        void OnDrop(Windows::UI::Xaml::DragEventArgs^ e) override;
        void OnDragOver(Windows::UI::Xaml::DragEventArgs^ e) override;

    private:
        TreeNode2^ rootNode;
        ViewModel^ flatViewModel;

    internal:
        TreeView2Item^ draggedTreeView2Item;
    };
}

