import { FormEvent, useEffect, useMemo, useState } from 'react';

type Field = {
  id: number;
  name: string;
  surfaceType: string;
  capacity: number;
};

type Reservation = {
  id: number;
  fieldId: number;
  fieldName: string;
  playerName: string;
  startTime: string;
  endTime: string;
  notes?: string;
};

type NewReservation = {
  fieldId: number;
  playerName: string;
  startTime: string;
  endTime: string;
  notes: string;
};

const initialForm: NewReservation = {
  fieldId: 0,
  playerName: '',
  startTime: '',
  endTime: '',
  notes: ''
};

export function App() {
  const [fields, setFields] = useState<Field[]>([]);
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [form, setForm] = useState<NewReservation>(initialForm);
  const [message, setMessage] = useState<string>('');

  const orderedReservations = useMemo(
    () => [...reservations].sort((a, b) => a.startTime.localeCompare(b.startTime)),
    [reservations]
  );

  useEffect(() => {
    void Promise.all([loadFields(), loadReservations()]);
  }, []);

  const loadFields = async () => {
    const response = await fetch('/api/fields');
    if (!response.ok) {
      throw new Error('Unable to load fields');
    }

    const data = (await response.json()) as Field[];
    setFields(data);
    if (data.length > 0) {
      setForm((prev) => ({ ...prev, fieldId: prev.fieldId || data[0].id }));
    }
  };

  const loadReservations = async () => {
    const response = await fetch('/api/reservations');
    if (!response.ok) {
      throw new Error('Unable to load reservations');
    }

    const data = (await response.json()) as Reservation[];
    setReservations(data);
  };

  const createReservation = async (event: FormEvent) => {
    event.preventDefault();
    setMessage('');

    const response = await fetch('/api/reservations', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        fieldId: Number(form.fieldId),
        playerName: form.playerName,
        startTime: new Date(form.startTime).toISOString(),
        endTime: new Date(form.endTime).toISOString(),
        notes: form.notes || null
      })
    });

    if (!response.ok) {
      const errorData = (await response.json()) as { error?: string };
      setMessage(errorData.error ?? 'Reservation failed.');
      return;
    }

    setMessage('Reservation created successfully.');
    setForm((prev) => ({ ...initialForm, fieldId: prev.fieldId }));
    await loadReservations();
  };

  return (
    <main className="page">
      <section className="card">
        <h1>Sport Field Reservations</h1>
        <p>Book your preferred field and keep schedules conflict-free.</p>

        <form onSubmit={createReservation} className="form-grid">
          <label>
            Field
            <select
              value={form.fieldId}
              onChange={(event) => setForm({ ...form, fieldId: Number(event.target.value) })}
              required
            >
              {fields.map((field) => (
                <option key={field.id} value={field.id}>
                  {field.name} ({field.surfaceType}, cap {field.capacity})
                </option>
              ))}
            </select>
          </label>

          <label>
            Player / Team Name
            <input
              value={form.playerName}
              onChange={(event) => setForm({ ...form, playerName: event.target.value })}
              placeholder="City Strikers"
              required
            />
          </label>

          <label>
            Start
            <input
              type="datetime-local"
              value={form.startTime}
              onChange={(event) => setForm({ ...form, startTime: event.target.value })}
              required
            />
          </label>

          <label>
            End
            <input
              type="datetime-local"
              value={form.endTime}
              onChange={(event) => setForm({ ...form, endTime: event.target.value })}
              required
            />
          </label>

          <label className="notes">
            Notes
            <textarea
              value={form.notes}
              onChange={(event) => setForm({ ...form, notes: event.target.value })}
              placeholder="Bring own balls"
            />
          </label>

          <button type="submit">Create Reservation</button>
        </form>

        {message && <p className="message">{message}</p>}
      </section>

      <section className="card">
        <h2>Upcoming Reservations</h2>
        <ul className="reservations">
          {orderedReservations.map((reservation) => (
            <li key={reservation.id}>
              <div>
                <strong>{reservation.fieldName}</strong> — {reservation.playerName}
              </div>
              <div>
                {new Date(reservation.startTime).toLocaleString()} to{' '}
                {new Date(reservation.endTime).toLocaleString()}
              </div>
              {reservation.notes && <div className="notes-text">{reservation.notes}</div>}
            </li>
          ))}
          {orderedReservations.length === 0 && <li>No reservations yet.</li>}
        </ul>
      </section>
    </main>
  );
}
