#pragma once
#include "pch.h"
#include "TreeNode2.h"

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;

namespace TreeViewControl {

    TreeNode2::TreeNode2()
    {
        childrenVector->VectorChanged += ref new VectorChangedEventHandler<TreeNode2 ^>(this, &TreeNode2::ChildrenVectorChanged);
    }

    void TreeNode2::Append(Object^ value)
    {
        int count = childrenVector->Size;
        TreeNode2^ targetNode = (TreeNode2^)value;
        targetNode->ParentNode = this;
        childrenVector->Append(targetNode);

        //If the count was 0 before we appended, then the HasItems property needs to change.
        if (count == 0)
        {
            this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("HasItems"));
        }

        this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("Size"));
    }

    void TreeNode2::Clear()
    {
        int count = childrenVector->Size;
        TreeNode2^ childNode;
        for (int i = 0; i < (int)Size; i++)
        {
            childNode = (TreeNode2^)GetAt(i);
            childNode->ParentNode = nullptr;
        }

        childrenVector->Clear();

        //If the count was not 0 before we cleared, then the HasItems property needs to change.
        if (count != 0)
        {
            this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("HasItems"));
        }

        this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("Size"));
    }

    IBindableIterator^ TreeNode2::First()
    {
        return dynamic_cast<IBindableIterator^>(childrenVector->First());
    }

    Object^ TreeNode2::GetAt(unsigned int index)
    {
        if ((int)index > -1 && index < childrenVector->Size)
        {
            return childrenVector->GetAt(index);
        }

        return nullptr;
    }

    IBindableVectorView^ TreeNode2::GetView()
    {
        return safe_cast<IBindableVectorView^>(childrenVector->GetView());
    }

    bool TreeNode2::IndexOf(Object^ value, unsigned int* index)
    {
        return childrenVector->IndexOf((TreeNode2^)value, index);
    }

    void TreeNode2::InsertAt(unsigned int index, Object^ value)
    {
        if ((int)index > -1 && index <= childrenVector->Size)
        {
            int count = childrenVector->Size;
            TreeNode2^ targetNode = (TreeNode2^)value;
            targetNode->ParentNode = this;
            return childrenVector->InsertAt(index, (TreeNode2^)value);

            //If the count was 0 before we insert, then the HasItems property needs to change.
            if (count == 0)
            {
                this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("HasItems"));
            }

            this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("Size"));
        }
    }

    void TreeNode2::RemoveAt(unsigned int index)
    {
        if ((int)index > -1 && index < childrenVector->Size)
        {
            int count = childrenVector->Size;
            TreeNode2^ targetNode = childrenVector->GetAt(index);
            targetNode->ParentNode = nullptr;
            childrenVector->RemoveAt(index);

            //If the count was 1 before we remove, then the HasItems property needs to change.
            if (count == 1)
            {
                this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("HasItems"));
            }

            this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("Size"));
        }
    }

    void TreeNode2::RemoveAtEnd()
    {
        int count = childrenVector->Size;
        TreeNode2^ targetNode = childrenVector->GetAt(childrenVector->Size - 1);
        targetNode->ParentNode = nullptr;
        childrenVector->RemoveAtEnd();

        //If the count was 1 before we remove, then the HasItems property needs to change.
        if (count == 1)
        {
            this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("HasItems"));
        }

        this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("Size"));
    }

    void TreeNode2::SetAt(unsigned int index, Object^ value)
    {
        if ((int)index > -1 && index <= childrenVector->Size)
        {
            childrenVector->GetAt(index)->ParentNode = nullptr;
            TreeNode2^ targetNode = (TreeNode2^)value;
            targetNode->ParentNode = this;
            return childrenVector->SetAt(index, targetNode);
        }
    }

    void TreeNode2::ChildrenVectorChanged(IObservableVector<TreeNode2^>^ sender, IVectorChangedEventArgs^ e)
    {
        VectorChanged(this, e);
    }

    unsigned int TreeNode2::Size::get()
    {
        return childrenVector->Size;
    }

    Object^ TreeNode2::Data::get()
    {
        return data;
    }

    void TreeNode2::Data::set(Object^ value)
    {
        data = value;
        this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("Data"));
    }

    TreeNode2^ TreeNode2::ParentNode::get()
    {
        return parentNode;
    }

    void TreeNode2::ParentNode::set(TreeNode2^ value)
    {
        parentNode = value;
        this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("ParentNode"));
        this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("Depth"));
    }

    bool TreeNode2::IsExpanded::get()
    {
        return isExpanded;
    }

    void TreeNode2::IsExpanded::set(bool value)
    {
        isExpanded = value;
        this->PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs("IsExpanded"));
    }

    bool TreeNode2::HasItems::get()
    {
        return (Size != 0);
    }

    int TreeNode2::Depth::get()
    {
        TreeNode2^ ancestorNode = this;
        int depth = -1;
        while ((ancestorNode->ParentNode) != nullptr)
        {
            depth++;
            ancestorNode = ancestorNode->ParentNode;
        }

        return depth;
    }

}