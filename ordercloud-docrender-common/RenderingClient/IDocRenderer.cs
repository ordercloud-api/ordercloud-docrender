using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderCloud.DocRender.common
{
	public interface IDocRenderer
	{
		Task<JobRenderResponse> SubmitRenderJobAsync(Dictionary<string, string> DocVars, Func<string, Task> moveFileOpAsync);
	}
}