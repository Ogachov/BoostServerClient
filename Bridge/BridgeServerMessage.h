#pragma once

#include <cstdio>
#include <cstdlib>
#include <cstring>

class chat_message
{
public:
    enum { header_length = 4 };
    enum { max_body_length = 512 };

    chat_message()
        : body_length_(0)
    {
    }

    const char* data() const
    {
        return data_;
    }

    char* data()
    {
        return data_;
    }

    std::size_t length() const
    {
        return header_length + body_length_;
    }

    const char* body() const
    {
        return data_ + header_length;
    }

    char* body()
    {
        return data_ + header_length;
    }

    std::size_t body_length() const
    {
        return body_length_;
    }

    void body_length(std::size_t new_length)
    {
        body_length_ = new_length;
        if (body_length_ > max_body_length)
            body_length_ = max_body_length;
    }

    bool decode_header()
    {
        body_length_ = *reinterpret_cast<int*>(data_);

        if (body_length_ > max_body_length)
        {
            body_length_ = 0;
            return false;
        }
        return true;
    }

    void encode_header()
    {
        *reinterpret_cast<int*>(data_) = body_length_;
    }

    int decode_command() const
    {
        return *reinterpret_cast<int*>(data_[4]);
    }

private:
    char data_[header_length + max_body_length];
    std::size_t body_length_;
};
