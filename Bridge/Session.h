#pragma once

#include <memory>
#include <iostream>
#include <set>
#include <boost/asio.hpp>

using boost::asio::ip::tcp;

class Participant
{
public:
    virtual ~Participant(){}
    virtual void deliver(const char* data, int32_t length) = 0;
};

typedef std::shared_ptr<Participant> ParticipantPtr;

class Session : public Participant, public std::enable_shared_from_this<Session>
{
private:
    tcp::socket socket_;
    enum { header_length = 4 };
    enum { buffer_length = 8192 };
    int32_t body_length_;
    int32_t sent_length_;
    char buffer_[header_length + buffer_length];

    std::set<ParticipantPtr> participants_;

public:
    Session(tcp::socket socket) : socket_(std::move(socket))
    {
        body_length_ = 0;
        sent_length_ = 0;
    }

    void deliver(const char* data, int32_t length) override
    {
        
    }

    void start()
    {
        do_read_header();
    }

private:
    void do_read_header()
    {
        auto self(shared_from_this());
        boost::asio::async_read(socket_,
            boost::asio::buffer(&buffer_[0], header_length),
            [this, self](boost::system::error_code ec, std::size_t)
        {
            if (!ec)
            {
                body_length_ = *reinterpret_cast<int32_t*>(&buffer_[0]);
                if (body_length_ > buffer_length)
                {
                    std::cout << "body length too large" << std::endl;
                }
                else
                {
                    do_read_body();
                }
            }
            else
            {
                std::cout << "read header error:" << ec.message() << std::endl;
            }
        });
    }

    void do_read_body()
    {
        auto self(shared_from_this());
        boost::asio::async_read(socket_,
            boost::asio::buffer(&buffer_[header_length], body_length_),
            [this, self](boost::system::error_code ec, std::size_t)
        {
            if (!ec)
            {
                
            }
        });
    }
};
