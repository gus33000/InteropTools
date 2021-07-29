#pragma once
#include "pch.h"
#include "TreeView2ItemAutomationPeer.h"
#include "TreeNode2.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Automation;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Data;


namespace TreeViewControl {
    //IExpandCollapseProvider
    Windows::UI::Xaml::Automation::ExpandCollapseState TreeView2ItemAutomationPeer::ExpandCollapseState::get()
    {
        Windows::UI::Xaml::Automation::ExpandCollapseState currentState = Windows::UI::Xaml::Automation::ExpandCollapseState::Collapsed;
        Windows::UI::Xaml::Controls::ListView^ ancestorListView = GetParentListView((DependencyObject^)Owner);

        TreeNode2^ targetNode;
        TreeNode2^ targetParentNode;

        if (ancestorListView)
        {
            TreeView2^ ancestorTreeView2 = (TreeView2^)ancestorListView;
            targetNode = (TreeNode2^)ancestorTreeView2->ItemFromContainer((TreeView2Item^)Owner);

            if (Owner->AllowDrop)
            {
                if (targetNode->IsExpanded)
                {
                    currentState = Windows::UI::Xaml::Automation::ExpandCollapseState::Expanded;
                }
                else
                {
                    currentState = Windows::UI::Xaml::Automation::ExpandCollapseState::Collapsed;
                }
            }
            else
            {
                currentState = Windows::UI::Xaml::Automation::ExpandCollapseState::LeafNode;
            }
        }

        return currentState;
    }

    void TreeView2ItemAutomationPeer::Collapse()
    {
        Windows::UI::Xaml::Controls::ListView^ ancestorListView = GetParentListView((DependencyObject^)Owner);

        if (ancestorListView)
        {
            TreeView2^ ancestorTreeView2 = (TreeView2^)ancestorListView;
            TreeNode2^ targetNode = (TreeNode2^)ancestorTreeView2->ItemFromContainer((TreeView2Item^)Owner);
            ancestorTreeView2->CollapseNode(targetNode);
            RaiseExpandCollapseAutomationEvent(Windows::UI::Xaml::Automation::ExpandCollapseState::Collapsed);
        }
    }

    void TreeView2ItemAutomationPeer::Expand()
    {
        Windows::UI::Xaml::Controls::ListView^ ancestorListView = GetParentListView((DependencyObject^)Owner);

        if (ancestorListView)
        {
            TreeView2^ ancestorTreeView2 = (TreeView2^)ancestorListView;
            TreeNode2^ targetNode = (TreeNode2^)ancestorTreeView2->ItemFromContainer((TreeView2Item^)Owner);
            ancestorTreeView2->ExpandNode(targetNode);
            RaiseExpandCollapseAutomationEvent(Windows::UI::Xaml::Automation::ExpandCollapseState::Expanded);
        }
    }

    void TreeView2ItemAutomationPeer::RaiseExpandCollapseAutomationEvent(Windows::UI::Xaml::Automation::ExpandCollapseState newState)
    {
        Windows::UI::Xaml::Automation::ExpandCollapseState oldState;

        if (newState == Windows::UI::Xaml::Automation::ExpandCollapseState::Expanded)
        {
            oldState = Windows::UI::Xaml::Automation::ExpandCollapseState::Collapsed;
        }
        else
        {
            oldState = Windows::UI::Xaml::Automation::ExpandCollapseState::Expanded;
        }

        RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers::ExpandCollapseStateProperty, oldState, newState);
    }

    //Position override

    //These methods are being overridden so that the TreeView2 under narrator reads out
    //the position of an item as compared to it's children, not it's overall position
    //in the listview. I've included an override for level as well, to give context on
    //how deep in the tree an item is.
    int TreeView2ItemAutomationPeer::GetSizeOfSetCore()
    {
        Windows::UI::Xaml::Controls::ListView^ ancestorListView = GetParentListView((DependencyObject^)Owner);

        TreeNode2^ targetNode;
        TreeNode2^ targetParentNode;

        int setSize = 0;

        if (ancestorListView)
        {
            TreeView2^ ancestorTreeView2 = (TreeView2^)ancestorListView;
            targetNode = (TreeNode2^)ancestorTreeView2->ItemFromContainer((TreeView2Item^)Owner);
            targetParentNode = targetNode->ParentNode;
            setSize = targetParentNode->Size;
        }

        return setSize;
    }

    int TreeView2ItemAutomationPeer::GetPositionInSetCore()
    {
        Windows::UI::Xaml::Controls::ListView^ ancestorListView = GetParentListView((DependencyObject^)Owner);

        TreeNode2^ targetNode;
        TreeNode2^ targetParentNode;

        int positionInSet = 0;

        if (ancestorListView)
        {
            TreeView2^ ancestorTreeView2 = (TreeView2^)ancestorListView;
            targetNode = (TreeNode2^)ancestorTreeView2->ItemFromContainer((TreeView2Item^)Owner);
            unsigned int positionInt;
            targetParentNode = targetNode->ParentNode;
            targetParentNode->IndexOf(targetNode, &positionInt);
            positionInSet = (int)positionInt + 1;
        }

        return positionInSet;
    }

    int TreeView2ItemAutomationPeer::GetLevelCore()
    {
        Windows::UI::Xaml::Controls::ListView^ ancestorListView = GetParentListView((DependencyObject^)Owner);

        TreeNode2^ targetNode;
        TreeNode2^ targetParentNode;

        int levelValue = 0;
        
        if (ancestorListView)
        {
            TreeView2^ ancestorTreeView2 = (TreeView2^)ancestorListView;
            targetNode = (TreeNode2^)ancestorTreeView2->ItemFromContainer((TreeView2Item^)Owner);
            levelValue = targetNode->Depth + 1;
        }

        return levelValue;
    }

    Platform::Object^ TreeView2ItemAutomationPeer::GetPatternCore(Windows::UI::Xaml::Automation::Peers::PatternInterface patternInterface)
    {
        if (patternInterface == Windows::UI::Xaml::Automation::Peers::PatternInterface::ExpandCollapse)
        {
            return this;
        }

        return ListViewItemAutomationPeer::GetPatternCore(patternInterface);
    }

    Windows::UI::Xaml::Controls::ListView^ TreeView2ItemAutomationPeer::GetParentListView(DependencyObject^ Owner)
    {
        DependencyObject^ treeViewItemAncestor = Owner;
        Windows::UI::Xaml::Controls::ListView^ ancestorListView = nullptr;
        while (treeViewItemAncestor != nullptr && ancestorListView == nullptr)
        {
            treeViewItemAncestor = Windows::UI::Xaml::Media::VisualTreeHelper::GetParent(treeViewItemAncestor);
            ancestorListView = dynamic_cast<Windows::UI::Xaml::Controls::ListView^>(treeViewItemAncestor);
        }

        return ancestorListView;
    }
}