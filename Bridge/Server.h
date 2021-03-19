#pragma once
#include <iostream>
#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/thread.hpp>

namespace asio = boost::asio;
using asio::ip::tcp;

class Server
{
    asio::io_service io_service_;
    tcp::acceptor acceptor_;
    tcp::socket socket_;
    asio::streambuf receive_buf_;

public:
    Server(int port) : acceptor_(io_service_, tcp::endpoint(tcp::v4(), port)), socket_(io_service_)
    {

    }

    void start()
    {
        start_accept();
        auto t = boost::thread(boost::bind(&boost::asio::io_service::run, &io_service_));
        t.detach();
    }

private:
    void start_accept()
    {
        std::cout << "waiting for connect" << std::endl;
        acceptor_.async_accept(socket_, boost::bind(&Server::on_accept, this, asio::placeholders::error));
    }

    void on_accept(const boost::system::error_code& error)
    {
        if (error)
        {
            std::cout << "accept failed:" << error.message() << std::endl;
            return;
        }

        std::cout << "accept" << std::endl;
    }
};