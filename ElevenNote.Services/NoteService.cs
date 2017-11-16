using ElevenNote.Data;
using ElevenNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services
{
    public class NoteService
    {
        private readonly Guid _userId;

        public NoteService(Guid userid)
        {
            _userId = userid;
        }

        public IEnumerable<NoteListItemModel> GetNotes()
        {
            using (var ctx = new ElevenNoteDbContext())
            {
                return
                    ctx.Notes
                    .Where(e => e.OwnerId == _userId)
                    .Select(
                        e => new NoteListItemModel
                        {
                            NoteId = e.NoteId,
                            Title = e.Title,
                            CreatedUtc = e.CreatedUtc,
                            ModifiedUtc = e.ModifiedUtc
                        })
                    .ToArray();
            }
        }

        public bool CreateNote(NoteCreateModel model)
        {
            using (var ctx = new ElevenNoteDbContext())
            {
                var entity = new NoteEntity
                {
                    OwnerId = _userId,
                    Title = model.Title,
                    Content = model.Content,
                    CreatedUtc = DateTime.UtcNow
                };

                ctx.Notes.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public NoteDetailModel GetNoteById(int id)
        {
            NoteEntity entity;

            using (var ctx = new ElevenNoteDbContext())
            {
                entity = GetNoteById(ctx, id);
            }

            if (entity == null) return new NoteDetailModel();

            return
                new NoteDetailModel
                {
                    NodeId = entity.NoteId,
                    Content = entity.Content,
                    Title = entity.Title,
                    CreatedUtc = entity.CreatedUtc,
                    ModifiedUtc = entity.ModifiedUtc
                };

        }

        public bool UpdateNote(NoteEditModel model)
        {
            using (var ctx = new ElevenNoteDbContext())
            {
                var entity = GetNoteById(ctx, model.NodeId);
                

                if (entity == null) return false;

                entity.Title = model.Title;
                entity.Content = model.Content;
                entity.ModifiedUtc = DateTime.UtcNow;

                return ctx.SaveChanges() == 1;
            }
        }

        private NoteEntity GetNoteById(ElevenNoteDbContext ctx, int id)
        {
            return
                ctx
                .Notes
                .SingleOrDefault(e => e.NoteId == id && e.OwnerId == _userId);

        }

        public bool DeleteNote(int id)
        {
            using (var ctx = new ElevenNoteDbContext())
            {
                var entity = GetNoteById(ctx, id);

                if (entity == null) return false;

                ctx.Notes.Remove(entity);
                return ctx.SaveChanges() == 1;
            }
        }
    }
}
