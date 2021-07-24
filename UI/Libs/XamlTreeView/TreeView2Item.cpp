#pragma once
#include "pch.h"
#include "TreeView2Item.h"
#include "TreeView2ItemAutomationPeer.h"

using namespace Windows::UI::Xaml;
using namespace Platform;

namespace TreeViewControl {
    TreeView2Item::TreeView2Item()
    {
    }

    TreeView2Item::~TreeView2Item()
    {
    }

    Windows::UI::Xaml::Controls::ListView^ TreeView2Item::GetAncestorListView(TreeView2Item^ targetItem)
    {
        DependencyObject^ TreeView2ItemAncestor = (DependencyObject^)this;
        Windows::UI::Xaml::Controls::ListView^ ancestorListView = nullptr;
        while (TreeView2ItemAncestor != nullptr && ancestorListView == nullptr)
        {
            TreeView2ItemAncestor = Windows::UI::Xaml::Media::VisualTreeHelper::GetParent(TreeView2ItemAncestor);
            ancestorListView = dynamic_cast<Windows::UI::Xaml::Controls::ListView^>(TreeView2ItemAncestor);
        }
        return ancestorListView;
    }

    void TreeView2Item::OnDrop(Windows::UI::Xaml::DragEventArgs^ e)
    {
        if (e->AcceptedOperation == Windows::ApplicationModel::DataTransfer::DataPackageOperation::Move)
        {
            TreeView2Item^ droppedOnItem = (TreeView2Item^)this;

            Windows::UI::Xaml::Controls::ListView^ ancestorListView = GetAncestorListView(droppedOnItem);

            if (ancestorListView)
            {
                TreeView2^ ancestorTreeView2 = (TreeView2^)ancestorListView;
                TreeView2Item^ droppedItem = ancestorTreeView2->draggedTreeView2Item;
                TreeNode2^ droppedNode = (TreeNode2^)ancestorTreeView2->ItemFromContainer(droppedItem);
                TreeNode2^ droppedOnNode = (TreeNode2^)ancestorTreeView2->ItemFromContainer(droppedOnItem);

                //Remove the item that was dragged
                unsigned int removeIndex;
                droppedNode->ParentNode->IndexOf(droppedNode, &removeIndex);

                if (droppedNode != droppedOnNode)
                {
                    droppedNode->ParentNode->RemoveAt(removeIndex);

                    //Append the dragged dropped item as a child of the node it was dropped onto
                    droppedOnNode->Append(droppedNode);

                    //If not set to true then the Reorder code of listview wil override what is being done here.
                    e->Handled = true;
                }
                else
                {
                    e->AcceptedOperation = Windows::ApplicationModel::DataTransfer::DataPackageOperation::None;
                }
            }
        }
    }

    void TreeView2Item::OnDragEnter(Windows::UI::Xaml::DragEventArgs^ e)
    {
        TreeView2Item^ draggedOverItem = (TreeView2Item^)this;

        e->AcceptedOperation = Windows::ApplicationModel::DataTransfer::DataPackageOperation::None;

        Windows::UI::Xaml::Controls::ListView^ ancestorListView = GetAncestorListView(draggedOverItem);

        if (ancestorListView)
        {
            TreeView2^ ancestorTreeView2 = (TreeView2^)ancestorListView;
            TreeView2Item^ draggedTreeView2Item = ancestorTreeView2->draggedTreeView2Item;
            TreeNode2^ draggedNode = (TreeNode2^)ancestorTreeView2->ItemFromContainer(draggedTreeView2Item);
            TreeNode2^ draggedOverNode = (TreeNode2^)ancestorTreeView2->ItemFromContainer(draggedOverItem);
            TreeNode2^ walkNode = draggedOverNode->ParentNode;

            while (walkNode != nullptr && walkNode != draggedNode)
            {
                walkNode = walkNode->ParentNode;
            }

            if (walkNode != draggedNode && draggedNode != draggedOverNode)
            {
                e->AcceptedOperation = Windows::ApplicationModel::DataTransfer::DataPackageOperation::Move;
            }
        }
    }

    void TreeView2Item::OnDragOver(Windows::UI::Xaml::DragEventArgs^ e)
    {
        e->DragUIOverride->IsGlyphVisible = true;
        e->AcceptedOperation = Windows::ApplicationModel::DataTransfer::DataPackageOperation::None;

        TreeView2Item^ draggedOverItem = (TreeView2Item^)this;

        Windows::UI::Xaml::Controls::ListView^ ancestorListView = GetAncestorListView(draggedOverItem);
        if (ancestorListView)
        {
            TreeView2^ ancestorTreeView2 = (TreeView2^)ancestorListView;
            TreeView2Item^ draggedTreeView2Item = ancestorTreeView2->draggedTreeView2Item;
            TreeNode2^ draggedNode = (TreeNode2^)ancestorTreeView2->ItemFromContainer(draggedTreeView2Item);
            TreeNode2^ draggedOverNode = (TreeNode2^)ancestorTreeView2->ItemFromContainer(draggedOverItem);
            TreeNode2^ walkNode = draggedOverNode->ParentNode;

            while (walkNode != nullptr && walkNode != draggedNode)
            {
                walkNode = walkNode->ParentNode;
            }

            if (walkNode != draggedNode && draggedNode != draggedOverNode)
            {
                e->AcceptedOperation = Windows::ApplicationModel::DataTransfer::DataPackageOperation::Move;
            }
        }
    }

    Windows::UI::Xaml::Automation::Peers::AutomationPeer^ TreeView2Item::OnCreateAutomationPeer()
    {
        return ref new TreeView2ItemAutomationPeer(this);
    }
}