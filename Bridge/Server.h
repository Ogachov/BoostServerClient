#pragma once
#include <iostream>
#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/thread.hpp>

namespace asio = boost::asio;
using asio::ip::tcp;

class Server
{
    asio::io_context io_context_;
    tcp::acceptor acceptor_;
    tcp::socket socket_;
    asio::streambuf receive_buf_;

public:
    Server(int port) : acceptor_(io_context_, tcp::endpoint(tcp::v4(), port)), socket_(io_context_)
    {
    }

    void start();

private:
    void do_accept();
};
