using API.Models.Dto;
using API.Models.Services;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly TodoRepository _todoRepository;
        public ToDoController(TodoRepository todoRepository )
        {
            _todoRepository = todoRepository;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            //mapping cuz use DTO & itsn't in Repository
            var todoList = _todoRepository.GetAll().Select(p => new ToDoItemDto
            {
                Id = p.Id,
                Text = p.Text,
                insertime = p.InsertTime,
                Links = new List<Links>() //HATEOAS implemention
                {
                     new Links
                     {
                          Href=Url.Action(nameof(Get),"ToDo",new {p.Id},Request.Scheme),
                          Rel="Self",
                          Method="Get"
                     },

                    new Links
                     {
                          Href=Url.Action(nameof(Delete),"ToDo",new {p.Id},Request.Scheme),
                          Rel="Delete",
                          Method="Delete"
                     },

                     new Links
                     {
                          Href=Url.Action(nameof(Edit),"ToDo",Request.Scheme),
                          Rel="Update",
                          Method="Put"
                     },
                }
            }).ToList();
            return Ok(todoList);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var todo = _todoRepository.Get(id);
            //mapping cuz property there isn't in repository
            return Ok(new ToDoItemDto
            {
                Id = todo.Id,
                insertime = todo.InsertTime,
                Text = todo.Text
            });
        }

        [HttpPost]
        public IActionResult Post([FromBody] ItemDto item)
        {
            var result = _todoRepository.Add(new AddToDoDto()
            {
                Todo = new TodoDto()
                {
                    Text = item.Text,
                }
            });

            string url = Url.Action(nameof(Get), "ToDo", new { Id = result.Todo.Id }, Request.Scheme);

            return Created(url, true);
        }

        
        [HttpPut()]
        public IActionResult Edit([FromBody] EditToDoDto editToDo)
        {
            var result = _todoRepository.Edit(editToDo);
            return Ok(result);
        }

        
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _todoRepository.Delete(id);
            return Ok();
        }
    }
}
