#pragma once
#include "pch.h"
#include "ViewModel.h"

using namespace Platform;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Data;

namespace TreeViewControl {
    ViewModel::ViewModel()
    {
        flatVectorRealizedItems->VectorChanged += ref new VectorChangedEventHandler<TreeNode2 ^>(this, &ViewModel::UpdateTreeView2);
    }

    void ViewModel::Append(Object^ value)
    {
        TreeNode2^ targetNode = (TreeNode2^)value;
        flatVectorRealizedItems->Append(targetNode);

        collectionChangedEventTokenVector.push_back(targetNode->VectorChanged += ref new BindableVectorChangedEventHandler(this, &ViewModel::TreeNode2VectorChanged));
        propertyChangedEventTokenVector.push_back(targetNode->PropertyChanged += ref new Windows::UI::Xaml::Data::PropertyChangedEventHandler(this, &ViewModel::TreeNode2PropertyChanged));
    }

    void ViewModel::Clear()
    {

        while (flatVectorRealizedItems->Size != 0)
        {
            RemoveAtEnd();
        }
    }

    IBindableIterator^ ViewModel::First()
    {
        return dynamic_cast<IBindableIterator^>(flatVectorRealizedItems->First());
    }

    Object^ ViewModel::GetAt(unsigned int index)
    {
        if ((int)index > -1 && (int)index < flatVectorRealizedItems->Size)
        {
            return flatVectorRealizedItems->GetAt(index);
        }

        return nullptr;
    }

    IBindableVectorView^ ViewModel::GetView()
    {
        return safe_cast<IBindableVectorView^>(flatVectorRealizedItems->GetView());
    }

    bool ViewModel::IndexOf(Object^ value, unsigned int* index)
    {
        return flatVectorRealizedItems->IndexOf((TreeNode2^)value, index);
    }

    void ViewModel::InsertAt(unsigned int index, Object^ value)
    {
        if ((int)index > -1 && (int)index <= flatVectorRealizedItems->Size)
        {
            TreeNode2^ targetNode = (TreeNode2^)value;
            flatVectorRealizedItems->InsertAt(index, targetNode);

            auto eventIndex = collectionChangedEventTokenVector.begin() + index;
            collectionChangedEventTokenVector.insert(eventIndex, targetNode->VectorChanged += ref new BindableVectorChangedEventHandler(this, &ViewModel::TreeNode2VectorChanged));

            eventIndex = propertyChangedEventTokenVector.begin() + index;
            propertyChangedEventTokenVector.insert(eventIndex,targetNode->PropertyChanged += ref new Windows::UI::Xaml::Data::PropertyChangedEventHandler(this, &ViewModel::TreeNode2PropertyChanged));
        }
    }

    void ViewModel::RemoveAt(unsigned int index)
    {
        if ((int)index > -1 && (int)index < flatVectorRealizedItems->Size)
        {
            TreeNode2^ targetNode = flatVectorRealizedItems->GetAt(index);
            flatVectorRealizedItems->RemoveAt(index);

            auto eventIndex = collectionChangedEventTokenVector.begin() + index;
            targetNode->VectorChanged -= collectionChangedEventTokenVector[index];
            collectionChangedEventTokenVector.erase(eventIndex);

            eventIndex = propertyChangedEventTokenVector.begin() + index;
            targetNode->PropertyChanged -= propertyChangedEventTokenVector[index];
            propertyChangedEventTokenVector.erase(eventIndex);
        }
    }

    void ViewModel::RemoveAtEnd()
    {
        int index = flatVectorRealizedItems->Size - 1;
        if (index >= 0)
        {
            TreeNode2^ targetNode = flatVectorRealizedItems->GetAt(index);
            flatVectorRealizedItems->RemoveAt(index);

            auto eventIndex = collectionChangedEventTokenVector.begin() + index;
            targetNode->VectorChanged -= collectionChangedEventTokenVector[index];
            collectionChangedEventTokenVector.erase(eventIndex);

            eventIndex = propertyChangedEventTokenVector.begin() + index;
            targetNode->PropertyChanged -= propertyChangedEventTokenVector[index];
            propertyChangedEventTokenVector.erase(eventIndex);
        }
    }

    void ViewModel::SetAt(unsigned int index, Object^ value)
    {
        if ((int)index > -1 && (int)index < flatVectorRealizedItems->Size)
        {
            TreeNode2^ targetNode = (TreeNode2^)value;
            TreeNode2^ removeNode = flatVectorRealizedItems->GetAt(index);
            flatVectorRealizedItems->SetAt(index, targetNode);

            auto eventIndex = collectionChangedEventTokenVector.begin() + index;
            removeNode->VectorChanged -= collectionChangedEventTokenVector[index];
            collectionChangedEventTokenVector.erase(eventIndex);
            collectionChangedEventTokenVector.insert(eventIndex, targetNode->VectorChanged += ref new BindableVectorChangedEventHandler(this, &ViewModel::TreeNode2VectorChanged));

            eventIndex = propertyChangedEventTokenVector.begin() + index;
            targetNode->PropertyChanged -= propertyChangedEventTokenVector[index];
            propertyChangedEventTokenVector.erase(eventIndex);
            propertyChangedEventTokenVector.insert(eventIndex, targetNode->PropertyChanged += ref new Windows::UI::Xaml::Data::PropertyChangedEventHandler(this, &ViewModel::TreeNode2PropertyChanged));
        }
    }

    void ViewModel::ExpandNode(TreeNode2^ targetNode)
    {
        if (!targetNode->IsExpanded)
        {
            targetNode->IsExpanded = true;
        }
    }

    void ViewModel::CollapseNode(TreeNode2^ targetNode)
    {
        if (targetNode->IsExpanded)
        {
            targetNode->IsExpanded = false;
        }
    }

    void ViewModel::AddNodeToView(TreeNode2^ targetNode, int index)
    {
        InsertAt(index, targetNode);
    }

    int ViewModel::AddNodeDescendantsToView(TreeNode2^ targetNode, int index, int offset)
    {
        if (targetNode->IsExpanded)
        {
            TreeNode2^ childNode;
            for (int i = 0; i < (int)targetNode->Size; i++)
            {
                childNode = (TreeNode2^)targetNode->GetAt(i);
                offset++;
                AddNodeToView(childNode, index + offset);
                offset = AddNodeDescendantsToView(childNode, index, offset);
            }

            return offset;
        }

        return offset;
    }

    void ViewModel::RemoveNodeAndDescendantsFromView(TreeNode2^ targetNode)
    {
        if (targetNode->IsExpanded)
        {
            TreeNode2^ childNode;
            for (int i = 0; i < (int)targetNode->Size; i++)
            {
                childNode = (TreeNode2^)targetNode->GetAt(i);
                RemoveNodeAndDescendantsFromView(childNode);
            }
        }

        int index = IndexOf(targetNode);
        RemoveAt(index);
    }

    int ViewModel::CountDescendants(TreeNode2^ targetNode)
    {
        int descendantCount = 0;
        TreeNode2^ childNode;
        for (int i = 0; i < (int)targetNode->Size; i++)
        {
            childNode = (TreeNode2^)targetNode->GetAt(i);
            descendantCount++;
            if (childNode->IsExpanded)
            {
                descendantCount = descendantCount + CountDescendants(childNode);
            }
        }

        return descendantCount;
    }

    int ViewModel::IndexOf(TreeNode2^ targetNode)
    {
        unsigned int index;
        bool isIndexed = IndexOf(targetNode, &index);
        if (isIndexed)
        {
            return (int)index;
        }
        else
        {
            return -1;
        }
    }

    void ViewModel::UpdateTreeView2(IObservableVector<TreeNode2^>^ sender, IVectorChangedEventArgs^ e)
    {
        VectorChanged(this, e);
    }

    void ViewModel::TreeNode2VectorChanged(IBindableObservableVector^ sender, Platform::Object^ e)
    {
        CollectionChange CC = ((IVectorChangedEventArgs^)e)->CollectionChange;
        switch (CC)
        {
            //Reset case, commonly seen when a TreeNode2 is cleared.
            //removes all nodes that need removing then 
            //toggles a collapse / expand to ensure order.
        case (CollectionChange::Reset) :
        {
            TreeNode2^ resetNode = (TreeNode2^)sender;
            int resetIndex = IndexOf(resetNode);
            if (resetIndex != Size - 1 && resetNode->IsExpanded)
            {
                TreeNode2^ childNode = resetNode;
                TreeNode2^ parentNode = resetNode->ParentNode;
                int stopIndex;
                bool isLastRelativeChild = true;
                while (parentNode != nullptr && isLastRelativeChild)
                {
                    unsigned int relativeIndex;
                    parentNode->IndexOf(childNode, &relativeIndex);
                    if (parentNode->Size - 1 != relativeIndex)
                    {
                        isLastRelativeChild = false;
                    }
                    else
                    {
                        childNode = parentNode;
                        parentNode = parentNode->ParentNode;
                    }
                }

                if (parentNode != nullptr)
                {
                    unsigned int siblingIndex;
                    parentNode->IndexOf(childNode, &siblingIndex);
                    TreeNode2^ siblingNode = (TreeNode2^)parentNode->GetAt(siblingIndex + 1);
                    stopIndex = IndexOf(siblingNode);
                }
                else
                {
                    stopIndex = Size;
                }

                for (int i = stopIndex - 1; i > resetIndex; i--)
                {
                    if ((flatVectorRealizedItems->GetAt(i))->ParentNode == nullptr)
                    {
                        RemoveNodeAndDescendantsFromView(flatVectorRealizedItems->GetAt(i));
                    }
                }

                if (resetNode->IsExpanded)
                {
                    CollapseNode(resetNode);
                    ExpandNode(resetNode);
                }
            }

            break;
        }

                                       //Inserts the TreeNode2 into the correct index of the ViewModel
        case (CollectionChange::ItemInserted) :
        {
            //We will find the correct index of insertion by first checking if the
            //node we are inserting into is expanded. If it is we will start walking
            //down the tree and counting the open items. This is to ensure we place
            //the inserted item in the correct index. If along the way we bump into
            //the item being inserted, we insert there then return, because we don't
            //need to do anything further.
            unsigned int index = ((IVectorChangedEventArgs^)e)->Index;
            TreeNode2^ targetNode = (TreeNode2^)sender->GetAt(index);
            TreeNode2^ parentNode = targetNode->ParentNode;
            TreeNode2^ childNode;
            int parentIndex = IndexOf(parentNode);
            int allOpenedDescendantsCount = 0;
            if (parentNode->IsExpanded)
            {
                for (int i = 0; i < (int)parentNode->Size; i++)
                {
                    childNode = (TreeNode2^)parentNode->GetAt(i);
                    if (childNode == targetNode)
                    {
                        AddNodeToView(targetNode, (parentIndex + i + 1 + allOpenedDescendantsCount));
                        if (targetNode->IsExpanded)
                        {
                            AddNodeDescendantsToView(targetNode, parentIndex + i + 1, allOpenedDescendantsCount);
                        }

                        return;
                    }

                    if (childNode->IsExpanded)
                    {
                        allOpenedDescendantsCount += CountDescendants(childNode);
                    }
                }

                AddNodeToView(targetNode, (parentIndex + parentNode->Size + allOpenedDescendantsCount));
                if (targetNode->IsExpanded)
                {
                    AddNodeDescendantsToView(targetNode, parentIndex + parentNode->Size, allOpenedDescendantsCount);
                }
            }

            break;
        }

                                              //Removes a node from the ViewModel when a TreeNode2
                                              //removes a child.
        case (CollectionChange::ItemRemoved) :
        {
            TreeNode2^ removeNode = (TreeNode2^)sender;
            int removeIndex = IndexOf(removeNode);

            if (removeIndex != Size - 1 && removeNode->IsExpanded)
            {
                TreeNode2^ childNode = removeNode;
                TreeNode2^ parentNode = removeNode->ParentNode;
                int stopIndex;
                bool isLastRelativeChild = true;
                while (parentNode != nullptr && isLastRelativeChild)
                {
                    unsigned int relativeIndex;
                    parentNode->IndexOf(childNode, &relativeIndex);
                    if (parentNode->Size - 1 != relativeIndex)
                    {
                        isLastRelativeChild = false;
                    }
                    else
                    {
                        childNode = parentNode;
                        parentNode = parentNode->ParentNode;
                    }
                }

                if (parentNode != nullptr)
                {
                    unsigned int siblingIndex;
                    parentNode->IndexOf(childNode, &siblingIndex);
                    TreeNode2^ siblingNode = (TreeNode2^)parentNode->GetAt(siblingIndex + 1);
                    stopIndex = IndexOf(siblingNode);
                }
                else
                {
                    stopIndex = Size;
                }

                for (int i = stopIndex - 1; i > removeIndex; i--)
                {
                    if ((flatVectorRealizedItems->GetAt(i))->ParentNode == nullptr)
                    {
                        RemoveNodeAndDescendantsFromView(flatVectorRealizedItems->GetAt(i));
                    }
                }
            }

            break;
        }

                                             //Triggered by a replace such as SetAt.
                                             //Updates the TreeNode2 that changed in the ViewModel.
        case (CollectionChange::ItemChanged) :
        {
            unsigned int index = ((IVectorChangedEventArgs^)e)->Index;
            TreeNode2^ targetNode = (TreeNode2^)sender->GetAt(index);
            TreeNode2^ parentNode = targetNode->ParentNode;
            TreeNode2^ childNode;
            int allOpenedDescendantsCount = 0;
            int parentIndex = IndexOf(parentNode);

            for (int i = 0; i < (int)parentNode->Size; i++)
            {
                childNode = (TreeNode2^)parentNode->GetAt(i);
                if (childNode->IsExpanded)
                {
                    allOpenedDescendantsCount += CountDescendants(childNode);
                }
            }

            TreeNode2^ removeNode = (TreeNode2^)GetAt(parentIndex + index + allOpenedDescendantsCount + 1);
            if (removeNode->IsExpanded)
            {
                CollapseNode(removeNode);
            }

            RemoveAt(parentIndex + index + allOpenedDescendantsCount + 1);
            InsertAt(parentIndex + index + allOpenedDescendantsCount + 1, targetNode);

            break;
        }

        }
    }

    void ViewModel::TreeNode2PropertyChanged(Object^ sender, PropertyChangedEventArgs^ e)
    {        
        if (e->PropertyName == "IsExpanded")
        {
            TreeNode2^ targetNode = (TreeNode2^)sender;
            if (targetNode->IsExpanded)
            {
                if (targetNode->Size != 0)
                {
                    int openedDescendantOffset = 0;
                    int index = IndexOf(targetNode);
                    index = index + 1;
                    TreeNode2^ childNode;
                    for (int i = 0; i < (int)targetNode->Size; i++)
                    {
                        childNode = (TreeNode2^)targetNode->GetAt(i);
                        AddNodeToView(childNode, ((int)index + i + openedDescendantOffset));
                        openedDescendantOffset = AddNodeDescendantsToView(childNode, (index + i), openedDescendantOffset);
                    }
                }
            }
            else
            {
                TreeNode2^ childNode;
                for (int i = 0; i < (int)targetNode->Size; i++)
                {
                    childNode = (TreeNode2^)targetNode->GetAt(i);
                    RemoveNodeAndDescendantsFromView(childNode);
                }
            }
        }
    }
}