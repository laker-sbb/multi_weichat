#include "Stdafx.h"
#pragma once

#include "Request.h"

using namespace System;
using namespace System::IO;

namespace CefSharp
{
    public interface class IRequestResponse
    {
        /// cancel the request, return nothing
        void Cancel();

        /// the current request
        property IRequest^ Request { IRequest^ get(); };

        /// respond with redirection to the provided URL
        void Redirect(String^ url);

        /// respond with data from Stream
        void RespondWith(Stream^ stream, String^ mimeType);

        void RespondWith(Stream^ stream, String^ mimeType, String^ statusText, int statusCode, IDictionary<String^, String^>^ responseHeaders);
    };

    enum class ResponseAction
    {
        Continue,
        Cancel,
        Redirect,
        Respond
    };

    ref class RequestResponse : IRequestResponse
    {
        IRequest^ _request;
        Stream^ _responseStream;
        String^ _mimeType;
        String^ _redirectUrl;
        ResponseAction _action;
        String^ _statusText;
        int _statusCode;
        IDictionary<String^, String^>^ _responseHeaders;

    internal:
        RequestResponse(IRequest^ request) :
            _action(ResponseAction::Continue),
                _request(request) {}

        property Stream^ ResponseStream { Stream^ get() { return _responseStream; } }
        property String^ MimeType { String^ get() { return _mimeType; } }
        property String^ StatusText { String^ get() { return _statusText; } }
        property int StatusCode { int get() { return _statusCode; } }
        property IDictionary<String^, String^>^ ResponseHeaders { IDictionary<String^, String^>^ get() { return _responseHeaders; } }
        property String^ RedirectUrl { String^ get() { return _redirectUrl; } }
        property ResponseAction Action { ResponseAction get() { return _action; } }

    public:
        virtual void Cancel();
        virtual property IRequest^ Request { IRequest^ get() { return _request; } }
        virtual void Redirect(String^ url);
        virtual void RespondWith(Stream^ stream, String^ mimeType);
        virtual void RespondWith(Stream^ stream, String^ mimeType, String^ statusText, int statusCode, IDictionary<String^, String^>^ responseHeaders);
    };
}