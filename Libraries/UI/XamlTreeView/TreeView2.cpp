#pragma once
#include "pch.h"
#include "TreeView2.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Controls;

namespace TreeViewControl {

    TreeView2::TreeView2()
    {
        flatViewModel = ref new ViewModel;
        rootNode = ref new TreeNode2();

        flatViewModel->ExpandNode(rootNode);

        CanReorderItems = true;
        AllowDrop = true;
        CanDragItems = true;

        rootNode->VectorChanged += ref new BindableVectorChangedEventHandler(flatViewModel, &ViewModel::TreeNode2VectorChanged);
        ItemClick += ref new Windows::UI::Xaml::Controls::ItemClickEventHandler(this, &TreeView2::TreeView2_OnItemClick);
        DragItemsStarting += ref new Windows::UI::Xaml::Controls::DragItemsStartingEventHandler(this, &TreeView2::TreeView2_DragItemsStarting);
        DragItemsCompleted += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Xaml::Controls::ListViewBase ^, Windows::UI::Xaml::Controls::DragItemsCompletedEventArgs ^>(this, &TreeView2::TreeView2_DragItemsCompleted);
        ItemsSource = flatViewModel;
    }

    void TreeView2::TreeView2_OnItemClick(Platform::Object^ sender, Windows::UI::Xaml::Controls::ItemClickEventArgs^ args)
    {
        TreeView2ItemClickEventArgs^ treeViewArgs = ref new TreeView2ItemClickEventArgs();
        treeViewArgs->ClickedItem = args->ClickedItem;

        TreeView2ItemClick(this, treeViewArgs);

        if (!treeViewArgs->IsHandled)
        {
            TreeNode2^ targetNode = (TreeNode2^)args->ClickedItem;
            if (targetNode->IsExpanded)
            {
                flatViewModel->CollapseNode(targetNode);
            }
            else
            {
                flatViewModel->ExpandNode(targetNode);                
            }
        }
    }

    void TreeView2::TreeView2_DragItemsStarting(Platform::Object^ sender, Windows::UI::Xaml::Controls::DragItemsStartingEventArgs^ e)
    {
        draggedTreeView2Item = (TreeView2Item^)this->ContainerFromItem(e->Items->GetAt(0));
    }

    void TreeView2::TreeView2_DragItemsCompleted(Windows::UI::Xaml::Controls::ListViewBase^ sender, Windows::UI::Xaml::Controls::DragItemsCompletedEventArgs^ args)
    {
        draggedTreeView2Item = nullptr;
    }

    void TreeView2::OnDrop(Windows::UI::Xaml::DragEventArgs^ e)
    {
        if (e->AcceptedOperation == Windows::ApplicationModel::DataTransfer::DataPackageOperation::Move)
        {
            Panel^ panel = this->ItemsPanelRoot;
            Windows::Foundation::Point point = e->GetPosition(panel);

            int aboveIndex = -1;
            int belowIndex = -1;
            unsigned int relativeIndex;

            IInsertionPanel^ insertionPanel = (IInsertionPanel^)panel;

            if (insertionPanel != nullptr)
            {
                insertionPanel->GetInsertionIndexes(point, &aboveIndex, &belowIndex);

                TreeNode2^ aboveNode = (TreeNode2^)flatViewModel->GetAt(aboveIndex);
                TreeNode2^ belowNode = (TreeNode2^)flatViewModel->GetAt(belowIndex);
                TreeNode2^ targetNode = (TreeNode2^)this->ItemFromContainer(draggedTreeView2Item);

                //Between two items
                if (aboveNode && belowNode)
                {
                    targetNode->ParentNode->IndexOf(targetNode, &relativeIndex);
                    targetNode->ParentNode->RemoveAt(relativeIndex);

                    if (belowNode->ParentNode == aboveNode)
                    {
                        aboveNode->InsertAt(0, targetNode);
                    }
                    else
                    {
                        aboveNode->ParentNode->IndexOf(aboveNode, &relativeIndex);
                        aboveNode->ParentNode->InsertAt(relativeIndex + 1, targetNode);
                    }
                }
                //Bottom of the list
                else if (aboveNode && !belowNode)
                {
                    targetNode->ParentNode->IndexOf(targetNode, &relativeIndex);
                    targetNode->ParentNode->RemoveAt(relativeIndex);

                    aboveNode->ParentNode->IndexOf(aboveNode, &relativeIndex);
                    aboveNode->ParentNode->InsertAt(relativeIndex + 1, targetNode);
                }
                //Top of the list
                else if (!aboveNode && belowNode)
                {
                    targetNode->ParentNode->IndexOf(targetNode, &relativeIndex);
                    targetNode->ParentNode->RemoveAt(relativeIndex);

                    rootNode->InsertAt(0, targetNode);
                }
            }
        }

        e->Handled = true;
        ListViewBase::OnDrop(e);
    }

    void TreeView2::OnDragOver(Windows::UI::Xaml::DragEventArgs^ e)
    {
        Windows::ApplicationModel::DataTransfer::DataPackageOperation savedOperation = Windows::ApplicationModel::DataTransfer::DataPackageOperation::None;

        e->AcceptedOperation = Windows::ApplicationModel::DataTransfer::DataPackageOperation::None;

        Panel^ panel = this->ItemsPanelRoot;
        Windows::Foundation::Point point = e->GetPosition(panel);

        int aboveIndex = -1;
        int belowIndex = -1;

        IInsertionPanel^ insertionPanel = (IInsertionPanel^)panel;

        if (insertionPanel != nullptr)
        {
            insertionPanel->GetInsertionIndexes(point, &aboveIndex, &belowIndex);

            if (aboveIndex > -1)
            {
                TreeNode2^ aboveNode = (TreeNode2^)flatViewModel->GetAt(aboveIndex);
                TreeNode2^ targetNode = (TreeNode2^)this->ItemFromContainer(draggedTreeView2Item);

                TreeNode2^ ancestorNode = aboveNode;
                while (ancestorNode != nullptr && ancestorNode != targetNode)
                {
                    ancestorNode = ancestorNode->ParentNode;
                }

                if (ancestorNode == nullptr)
                {
                    savedOperation = Windows::ApplicationModel::DataTransfer::DataPackageOperation::Move;
                    e->AcceptedOperation = Windows::ApplicationModel::DataTransfer::DataPackageOperation::Move;
                }
            }
            else
            {
                savedOperation = Windows::ApplicationModel::DataTransfer::DataPackageOperation::Move;
                e->AcceptedOperation = Windows::ApplicationModel::DataTransfer::DataPackageOperation::Move;
            }
        }

        ListViewBase::OnDragOver(e);
        e->AcceptedOperation = savedOperation;
    }

    void TreeView2::ExpandNode(TreeNode2^ targetNode)
    {
        flatViewModel->ExpandNode(targetNode);
    }

    void TreeView2::CollapseNode(TreeNode2^ targetNode)
    {
        flatViewModel->CollapseNode(targetNode);
    }

    void TreeView2::PrepareContainerForItemOverride(DependencyObject^ element, Object^ item)
    {
        ((UIElement^)element)->AllowDrop = true;

        ListView::PrepareContainerForItemOverride(element, item);
    }

    DependencyObject^ TreeView2::GetContainerForItemOverride()
    {
        TreeView2Item^ targetItem = ref new TreeView2Item();
        return (DependencyObject^)targetItem;
    }
}