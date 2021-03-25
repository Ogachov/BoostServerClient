#include "Server.h"
#include "Session.h"

void Server::start()
{
    do_accept();
    auto t = boost::thread(boost::bind(&boost::asio::io_context::run, &io_context_));
    t.detach();
}

void Server::do_accept()
{
    std::cout << "waiting for connect" << std::endl;
    acceptor_.async_accept(
        [this](boost::system::error_code ec, tcp::socket socket)
    {
        if (!ec)
        {
            std::cout << "accept:" << socket.local_endpoint().address().to_string() << ":" << socket.local_endpoint().port() << std::endl;
            std::make_shared<Session>(std::move(socket))->start();
            do_accept();
        }
    });
}
