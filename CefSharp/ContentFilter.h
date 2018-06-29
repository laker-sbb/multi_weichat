#include "include\cef_content_filter.h"

#include <iostream>
#include<fstream>


using namespace std;
namespace CefSharp
{
	
	class ContentFilter : public CefContentFilter {
	
	private:
		int browserid;
		string remainder_;
		//int flag;
		std::string responseContent;
		gcroot<IWebBrowser^> _browserControl;
		
		
		public:
			ContentFilter(int browserid){
				this->browserid = browserid;
			}
			~ContentFilter() { _browserControl = nullptr; }
			ContentFilter(IWebBrowser^ browserControl) : _browserControl(browserControl) {}

			virtual void ProcessData(const void* data, int data_size,
                            CefRefPtr<CefStreamReader>& substitute_data)
							override{
								IRequestHandler^ handler = _browserControl->RequestHandler;

								string look_for_user = "\"User\": {"; //identy user
								string look_for_contract = "\"ContactList\": [{"; //identy contract
								string look_for_msg = "\"AddMsgList\": [{";
								string look_for_nickname = "\"NickName\":";
								
								string data_copy(static_cast<const char*>(data),data_size);
								trim(data_copy);
								if(data_copy.at(0)=='{'  && data_copy.at(data_copy.length()-1)=='}')
								{
									//insertDB(data_copy);
									responseContent = data_copy.substr(0);
									String^ content = gcnew String(responseContent.c_str(),0,responseContent.size(),gcnew System::Text::UTF8Encoding);
										
									handler->GetResponseContent(content);
								}
								else if(data_copy.at(0)=='{')
								{
									//char tail = data_copy.at(data_copy.length()-1);
									remainder_ = data_copy.substr(0);
								}
								else if(data_copy.at(data_copy.length()-1)=='}')
								{
									//char tail = data_copy.at(data_copy.length()-1);
									if(!remainder_.empty())
									{
										remainder_+=data_copy;
										//insertDB(remainder_);
										//responseContent = remainder_.substr(0);
										String^ content = gcnew String(remainder_.c_str(),0,remainder_.size(),gcnew System::Text::UTF8Encoding);
										
										handler->GetResponseContent(content);
									}
								}
								else
								{
									if(!remainder_.empty())
									{
										remainder_+=data_copy;
									}
								}
								

								


			}

			virtual void Drain(CefRefPtr<CefStreamReader>& remainder) {
			
				
			}

			

			std::string trim(string& str) 
			{
					string::size_type pos = str.find_last_not_of('\n'); 
					if(pos != string::npos) 
					{ 
						str.erase(pos + 1); 
						pos = str.find_first_not_of('\n'); 
						if(pos != string::npos)
							str.erase(0, pos); 
					} 
					else
					{
						str.erase(str.begin(), str.end()); 
					}
					return str; 
			} 

			protected:
			IMPLEMENT_REFCOUNTING(ContentFilter);

	};
}